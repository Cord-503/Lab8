using UnityEngine;

public class CustomTerrainGenerator : MonoBehaviour
{
    public Terrain terrain;
    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    public float heightScale = 10f;
    public float noiseScale = 0.03f;
    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2f;

    void Start()
    {
        GenerateCustomTerrain();
    }

    void GenerateCustomTerrain()
    {
        float[,] heightMap = GenerateHeightMap();
        ApplyHeightMap(heightMap);
    }

    float[,] GenerateHeightMap()
    {
        float[,] heightMap = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency * noiseScale;
                    float sampleY = y / scale * frequency * noiseScale;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                // Add some mountains
                float distanceToCenter = Vector2.Distance(new Vector2(x, y), new Vector2(width / 2f, height / 2f));
                float mountainNoise = Mathf.PerlinNoise(x * 0.01f, y * 0.01f);
                float mountainHeight = Mathf.Max(0, 1 - distanceToCenter / (width * 0.4f)) * mountainNoise * 2;

                heightMap[x, y] = Mathf.Clamp01((noiseHeight + 1) / 2f + mountainHeight);
            }
        }

        return heightMap;
    }

    void ApplyHeightMap(float[,] heightMap)
    {
        TerrainData terrainData = terrain.terrainData;
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, heightScale, height);
        terrainData.SetHeights(0, 0, heightMap);
    }
}