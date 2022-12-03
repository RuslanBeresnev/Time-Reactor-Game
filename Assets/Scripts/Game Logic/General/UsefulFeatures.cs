using UnityEngine;

/// <summary>
/// Полезные функции, используемые в логике игры
/// </summary>
public static class UsefulFeatures
{
    /// <summary>
    /// Вычислить, насколько сильно входит объект в ближайшую поверхность (принадлежащую маске) по направлению луча от точки начала до точки конца
    /// (точки начала и конца должны принадлежать объекту; например, начало в центре объекта, а конец у некоторого края)
    /// </summary>
    public static float CalculateDepthOfObjectEntryIntoNearestSurface(Vector3 startPoint, Vector3 endPoint, LayerMask layersToCheck)
    {
        RaycastHit hit;
        var objectStartToEndRay = new Ray(startPoint, (endPoint - startPoint).normalized);
        var distanceBetweenObjectStartAndEnd = Vector3.Distance(startPoint, endPoint);

        if (Physics.Raycast(objectStartToEndRay, out hit, distanceBetweenObjectStartAndEnd, layersToCheck))
        {
            return Vector3.Distance(hit.point, endPoint);
        }

        return 0f;
    }
}