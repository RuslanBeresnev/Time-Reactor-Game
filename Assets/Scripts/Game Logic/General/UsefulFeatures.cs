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

    /// <summary>
    /// Получить первый попавшийся компонент Entity в иерархии объекта. Если у текущего объекта нет компонента Entity,
    /// просматривается родительский объект и так далее до корня иерархии просматриваются все предки объекта, пока не
    /// будет найден компонент Entity. Если компонент не найден, будет возвращено null. Эта функция нужна для составных
    /// объектов из нескольких коллайдеров, когда компонент Entity располагается на самом верхнем в иерархии объекте.
    /// </summary>
    public static Entity GetFirstEntityComponentInObjectHierarchy(Transform obj)
    {
        var currentObj = obj;
        while (true)
        {
            var entityComponent = currentObj.GetComponent<Entity>();
            if (entityComponent != null)
            {
                return entityComponent;
            }
            currentObj = currentObj.parent;
            if (currentObj == null)
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Получить первый попавшийся компонент ObjectWithInformation в иерархии объекта. Если у текущего объекта
    /// нет компонента ObjectWithInformation, просматривается родительский объект и так далее до корня иерархии
    /// просматриваются все предки объекта, пока не будет найден компонент ObjectWithInformation. Если компонент
    /// не найден, будет возвращено null. Эта функция нужна для составных объектов из нескольких коллайдеров,
    /// когда компонент ObjectWithInformation располагается на самом верхнем в иерархии объекте.
    /// </summary>
    public static ObjectWithInformation GetFirstObjectWithInformationComponentInObjectHierarchy(Transform obj)
    {
        var currentObj = obj;
        while (true)
        {
            var objWithInfoComponent = currentObj.GetComponent<ObjectWithInformation>();
            if (objWithInfoComponent != null)
            {
                return objWithInfoComponent;
            }
            currentObj = currentObj.parent;
            if (currentObj == null)
            {
                return null;
            }
        }
    }
}