using System;
using RoomInteriorGenerator;
using System.Linq;
using Microsoft.FSharp.Core;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Процедурная генерация интерьера в комнатах
/// </summary>
public class GenerateInterior : MonoBehaviour
{
    [SerializeField] private int roomLength;
    [SerializeField] private int roomWidth;
    [SerializeField] private DataTableRowWrapper[] dataTable;
    [SerializeField] private int maximumAmountOfObjects;
    private readonly int floorNumber = GameProperties.FloorNumber;

    private void Start()
    {
        var dataTableRows = dataTable.Select(row => row.ToDataTableRow()).ToArray();
        var dataTableFs = new DataTable.DataTable<GameObject>(dataTableRows);

        // Приведение функции размещения к правильному F# типу
        var tuplePlacementFunction =
            FuncConvert
                .ToFSharpFunc<Tuple<DataTable.DataTableRow<GameObject>, DataTable.ObjectVariant<GameObject>, int, int>>(
                    t => PlaceObject(t.Item1, t.Item2, t.Item3, t.Item4));
        var placementFunction = FuncConvert.FuncFromTupled(tuplePlacementFunction);

        var room = new Room<GameObject>(roomLength, roomWidth, floorNumber, dataTableFs);
        room.GenerateInterior(maximumAmountOfObjects, placementFunction);
    }

    /// <summary>
    /// Приведение правила расположения объекта к подходящему углу поворота по оси Y
    /// </summary>
    private static Quaternion ToQuaternion(DataTable.Rule placementRule) =>
        placementRule.ToString() switch
        {
            "Node None" => Quaternion.Euler(0, 180, 0),
            "Node AgainstTheBottomWall" => Quaternion.Euler(0, 0, 0),
            "Node AgainstTheTopWall" => Quaternion.Euler(0, 180, 0),
            "Node AgainstTheRightWall" => Quaternion.Euler(0, 270, 0),
            "Node AgainstTheLeftWall" => Quaternion.Euler(0, 90, 0),
            "Node InTheCorner" => Quaternion.Euler(0, 180, 0),
            "Leaf LeftTo" => Quaternion.Euler(0, 270, 0),
            "Leaf RightTo" => Quaternion.Euler(0, 90, 0),
            "Leaf Behind" => Quaternion.Euler(0, 0, 0),
            "Leaf InFrontOf" => Quaternion.Euler(0, 180, 0),
            "Leaf Anywhere" => Quaternion.Euler(0, 0, 0),
            _ => throw new InvalidOperationException($"Unknown placementRule value: {placementRule}")
        };

    /// <summary>
    /// Функция размещения объекта в комнате
    /// </summary>
    private void PlaceObject(DataTable.DataTableRow<GameObject> objectToPlace,
        DataTable.ObjectVariant<GameObject> objectToPlaceVariant, int rowIndex, int columnIndex)
    {
        var prefab = objectToPlaceVariant.Variant;
        var placementRule = objectToPlace.PlacementRule;

        var parentPosition = transform.position;
        var position = new Vector3(rowIndex + parentPosition.x, parentPosition.y, columnIndex + parentPosition.z);

        Instantiate(prefab, position, ToQuaternion(placementRule), transform);
    }

    /// <summary>
    /// Обертка для отображения DataTable в инспекторе
    /// </summary>
    [Serializable]
    public class DataTableRowWrapper
    {
        private enum PlacementRuleWrapper
        {
            WithoutRule,
            AgainstTheTopWall,
            AgainstTheBottomWall,
            AgainstTheLeftWall,
            AgainstTheRightWall,
            InTheCorner,
            LeftToParent,
            RightToParent,
            BehindParent,
            InFrontOfParent,
            AnywhereNearParent
        }

        [SerializeField] private string name;
        [SerializeField] private ObjectVariantWrapper[] variants;
        [SerializeField] private PlacementRuleWrapper placementRule;
        [SerializeField] private DataTableRowWrapper[] leafs;

        /// <summary>
        /// Приведение обертки PlacementRuleWrapper к библиотечному типу PlacementRule
        /// </summary>
        private DataTable.Rule ToPlacementRule(PlacementRuleWrapper rule) =>
            rule switch
            {
                PlacementRuleWrapper.WithoutRule => DataTable.Rule.NewNode(DataTable.NodePlacementRule.None),
                PlacementRuleWrapper.AgainstTheBottomWall => DataTable.Rule.NewNode(DataTable.NodePlacementRule
                    .AgainstTheBottomWall),
                PlacementRuleWrapper.AgainstTheTopWall => DataTable.Rule.NewNode(DataTable.NodePlacementRule
                    .AgainstTheTopWall),
                PlacementRuleWrapper.AgainstTheRightWall => DataTable.Rule.NewNode(DataTable.NodePlacementRule
                    .AgainstTheRightWall),
                PlacementRuleWrapper.AgainstTheLeftWall => DataTable.Rule.NewNode(DataTable.NodePlacementRule
                    .AgainstTheLeftWall),
                PlacementRuleWrapper.InTheCorner => DataTable.Rule.NewNode(DataTable.NodePlacementRule.InTheCorner),
                PlacementRuleWrapper.LeftToParent => DataTable.Rule.NewLeaf(DataTable.LeafPlacementRule.LeftTo),
                PlacementRuleWrapper.RightToParent => DataTable.Rule.NewLeaf(DataTable.LeafPlacementRule.RightTo),
                PlacementRuleWrapper.BehindParent => DataTable.Rule.NewLeaf(DataTable.LeafPlacementRule.Behind),
                PlacementRuleWrapper.InFrontOfParent => DataTable.Rule.NewLeaf(DataTable.LeafPlacementRule.InFrontOf),
                PlacementRuleWrapper.AnywhereNearParent => DataTable.Rule.NewLeaf(DataTable.LeafPlacementRule.Anywhere),
                _ => throw new InvalidOperationException($"Unknown placementRule value: {rule}")
            };

        /// <summary>
        /// Приведение обертки DataTableRowWrapper к библиотечному типу DataTableRow
        /// </summary>
        public DataTable.DataTableRow<GameObject> ToDataTableRow()
        {
            var variantsArray = variants.Select(variant => variant.ToObjectVariant()).ToArray();
            var leafsDataTable = leafs.Select(row => row.ToDataTableRow()).ToArray();
            return new DataTable.DataTableRow<GameObject>(name, variantsArray, ToPlacementRule(placementRule),
                leafsDataTable);
        }
    }

    /// <summary>
    /// Обертка для отображения ObjectVariant в инспекторе
    /// </summary>
    [Serializable]
    public class ObjectVariantWrapper
    {
        [SerializeField] private GameObject variant;
        [SerializeField] private int freeCellsOnRight;
        [SerializeField] private int freeCellsOnLeft;
        [SerializeField] private int freeCellsOnTop;
        [SerializeField] private int freeCellsOnBottom;

        /// <summary>
        /// Приведение обертки ObjectVariantWrapper к библиотечному типу ObjectVariant
        /// </summary>
        public DataTable.ObjectVariant<GameObject> ToObjectVariant()
        {
            return new DataTable.ObjectVariant<GameObject>(variant, freeCellsOnLeft, freeCellsOnRight, freeCellsOnTop,
                freeCellsOnBottom);
        }
    }
}