using UnityEngine;

public class ShapeGenerator {
    readonly Mesh _mesh;
    readonly int _resolution;
    readonly Vector3 _localUp;
    readonly ShapeSettings _shapeSettings;
    readonly Vector3 _axisA;
    readonly Vector3 _axisB;
    readonly int _radius;
    readonly INoiseFilter[] _noiseFilters;

    public ShapeGenerator(Mesh mesh, int resolution, Vector3 localUp, ShapeSettings shapeSettings) {
        _mesh = mesh;
        _resolution = resolution;
        _localUp = localUp;
        _shapeSettings = shapeSettings;
        _radius = shapeSettings.radius;
        _axisA = new Vector3(_localUp.y, _localUp.z, _localUp.x);
        _axisB = Vector3.Cross(_localUp, _axisA);
        _noiseFilters = new INoiseFilter[_shapeSettings.noiseLayers.Length];
        for (int i = 0; i < _noiseFilters.Length; i++) {
            _noiseFilters[i] = INoiseFilter.Instance(_shapeSettings.noiseLayers[i].noiseSettings);
        }
    }

    /**
     *
     */
    Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere) {
        float elevation = 0;
        float firstLayerValue = 0;

        if (_noiseFilters.Length > 0) {
            firstLayerValue = _noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (_shapeSettings.noiseLayers[0].enabled) {
                elevation = firstLayerValue;
            }
        }

        for (int i = 1; i < _noiseFilters.Length; i++) {
            if (!_shapeSettings.noiseLayers[i].enabled) continue;
            float mask = _shapeSettings.noiseLayers[i].useFirstLayerAsMask ? firstLayerValue : 1;
            elevation += _noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
        }
        return pointOnUnitSphere * (_radius * (elevation + 1));
    }

    /**
     *
     */
    public void ConstructMesh() {
        int triangleIndex = 0;

        Vector3[] vertices = new Vector3[_resolution * _resolution];
        int[] triangle = new int[(_resolution - 1) * (_resolution - 1) * 6];

        for (int x = 0; x < _resolution; x++) {
            for (int y = 0; y < _resolution; y++) {
                int i = x + y * _resolution;
                Vector2 percent = new Vector2(x, y) / (_resolution - 1);
                Vector3 pointOnCube = _localUp + (percent.x - .5f) * 2 * _axisA + (percent.y - .5f) * 2 * _axisB;
                Vector3 pointOnSphere = pointOnCube.normalized;
                vertices[i] = CalculatePointOnPlanet(pointOnSphere);

                if (x == _resolution - 1 || y == _resolution - 1) continue;
                // Triangle 1
                triangle[triangleIndex] = i;
                triangle[triangleIndex + 1] = i + _resolution + 1;
                triangle[triangleIndex + 2] = i + _resolution;
                // Triangle 2
                triangle[triangleIndex + 3] = i;
                triangle[triangleIndex + 4] = i + 1;
                triangle[triangleIndex + 5] = i + _resolution + 1;

                triangleIndex += 6;
            }
        }
        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangle;
        _mesh.RecalculateNormals();
    }
}