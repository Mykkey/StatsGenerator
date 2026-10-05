using System.Collections.Generic;
using UnityEngine;

// Builds the baseball field as one vector-style mesh, so it stays sharp at any zoom.
// Shapes are drawn in the order they are added (later shapes cover earlier ones).
[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldBuilder : MonoBehaviour
{
    [Tooltip("Needs a material using the shader Universal Render Pipeline/2D/Sprite-Unlit-Default")]
    public Material material;
    [Tooltip("World units per design unit. 0.1 makes the field 140 x 100 units.")]
    public float scale = 0.1f;
    public int arcSegments = 256;
    public int sortingOrder = -10;

    // Design space matches the PNG/SVG: 1400 x 1000, y pointing down, home plate at (700, 910).
    const float W = 1400f, H = 1000f;
    static readonly Vector2 Home = new Vector2(700f, 910f);
    const float FenceR = 700f, GrassR = 655f, Side = 190f, K = 0.70710678f;

    static readonly Color Lawn = Rgb(40, 120, 50);
    static readonly Color LawnStripe = Rgb(46, 130, 56);
    static readonly Color Track = Rgb(150, 110, 60);
    static readonly Color Outfield = Rgb(55, 145, 65);
    static readonly Color Dirt = Rgb(190, 140, 80);
    static readonly Color InGrass = Rgb(50, 140, 60);
    static readonly Color Path = Rgb(235, 215, 170);
    static readonly Color Mound = Rgb(170, 120, 65);

    List<Vector3> verts = new List<Vector3>();
    List<Color> colors = new List<Color>();
    List<int> tris = new List<int>();

    static Color Rgb(int r, int g, int b) { return new Color(r / 255f, g / 255f, b / 255f, 1f); }

    void OnEnable() { Build(); }
    void OnValidate() { if (isActiveAndEnabled) Build(); }

    [ContextMenu("Rebuild Field")]
    public void Build()
    {
        verts.Clear(); colors.Clear(); tris.Clear();

        // Mowing stripes
        Quad(0, 0, W, H, Lawn);
        for (float x = 50; x < W; x += 100) Quad(x, 0, 50, H, LawnStripe);

        // Outfield: warning track, then grass, bounded by the foul lines
        Sector(Home, FenceR, 45, 135, Track);
        Sector(Home, GrassR, 45, 135, Outfield);

        // Infield
        Vector2 cx = new Vector2(Home.x, Home.y - Side * K);
        Disc(cx, Side * 0.95f, Dirt);
        Disc(Home, 40f, Dirt);

        Vector2 first = new Vector2(Home.x + Side * K, Home.y - Side * K);
        Vector2 third = new Vector2(Home.x - Side * K, Home.y - Side * K);
        Vector2 second = new Vector2(Home.x, Home.y - Side * 2f * K);
        Fan(new[] { Shrink(Home, cx), Shrink(first, cx), Shrink(second, cx), Shrink(third, cx) }, InGrass);

        // Foul lines
        Line(Home, Home + Polar(FenceR, 45), 4f, Color.white);
        Line(Home, Home + Polar(FenceR, 135), 4f, Color.white);

        // Base paths
        Line(Home, first, 3f, Path);
        Line(first, second, 3f, Path);
        Line(second, third, 3f, Path);
        Line(third, Home, 3f, Path);

        // Pitcher's mound
        Disc(cx, 24f, Mound);

        // Batter's boxes (outlines)
        Outline(Home.x - 38, Home.y - 18, 20, 36, 2f, Color.white);
        Outline(Home.x + 18, Home.y - 18, 20, 36, 2f, Color.white);

        Apply();
    }

    void Apply()
    {
        Mesh mesh = new Mesh { name = "Baseball Field" };
        if (verts.Count > 65000) mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetVertices(verts);
        // Vertex colors are not converted automatically, so do it in Linear color space.
        if (QualitySettings.activeColorSpace == ColorSpace.Linear)
            for (int i = 0; i < colors.Count; i++) colors[i] = colors[i].linear;
        mesh.SetColors(colors);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (material != null) mr.sharedMaterial = material;
        mr.sortingOrder = sortingOrder;
    }

    // ---- helpers (all inputs are in design space) ----

    // Design space (y down) -> local space (y up), centered on the field.
    Vector3 P(Vector2 d) { return new Vector3((d.x - W / 2f) * scale, (H / 2f - d.y) * scale, 0f); }

    // Math-style angle (counterclockwise, y up) -> design-space offset.
    static Vector2 Polar(float r, float deg)
    {
        float a = deg * Mathf.Deg2Rad;
        return new Vector2(r * Mathf.Cos(a), -r * Mathf.Sin(a));
    }

    static Vector2 Shrink(Vector2 p, Vector2 center) { return center + (p - center) * 0.72f; }

    void Tri(Vector2 a, Vector2 b, Vector2 c, Color col)
    {
        int i = verts.Count;
        verts.Add(P(a)); verts.Add(P(b)); verts.Add(P(c));
        colors.Add(col); colors.Add(col); colors.Add(col);
        tris.Add(i); tris.Add(i + 1); tris.Add(i + 2);
    }

    // Convex polygon as a triangle fan.
    void Fan(Vector2[] pts, Color col)
    {
        for (int i = 1; i < pts.Length - 1; i++) Tri(pts[0], pts[i], pts[i + 1], col);
    }

    void Quad(float x, float y, float w, float h, Color col)
    {
        Fan(new[] { new Vector2(x, y), new Vector2(x + w, y), new Vector2(x + w, y + h), new Vector2(x, y + h) }, col);
    }

    void Sector(Vector2 center, float r, float a0, float a1, Color col)
    {
        for (int i = 0; i < arcSegments; i++) {
            float t0 = Mathf.Lerp(a0, a1, i / (float)arcSegments);
            float t1 = Mathf.Lerp(a0, a1, (i + 1) / (float)arcSegments);
            Tri(center, center + Polar(r, t0), center + Polar(r, t1), col);
        }
    }

    void Disc(Vector2 center, float r, Color col) { Sector(center, r, 0, 360, col); }

    void Line(Vector2 a, Vector2 b, float width, Color col)
    {
        Vector2 dir = (b - a).normalized;
        Vector2 n = new Vector2(-dir.y, dir.x) * (width / 2f);
        Fan(new[] { a + n, b + n, b - n, a - n }, col);
    }

    void Outline(float x, float y, float w, float h, float t, Color col)
    {
        Quad(x, y, w, t, col);
        Quad(x, y + h - t, w, t, col);
        Quad(x, y, t, h, col);
        Quad(x + w - t, y, t, h, col);
    }
}
