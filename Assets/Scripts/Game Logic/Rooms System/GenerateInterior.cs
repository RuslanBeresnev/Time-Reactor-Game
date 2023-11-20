using System;
using RoomInteriorGenerator;
using System.Linq;
using Microsoft.FSharp.Core;
using UnityEditor.Experimental.RestService;
using UnityEngine;
using UnityEngine.UIElements;

public class GenerateInterior : MonoBehaviour
{
    [SerializeField] private int roomLength;
    [SerializeField] private int roomWidth;
    [SerializeField] private DataTableRowWrapper[] dataTable;
    [SerializeField] private int maximumAmountOfObjects;
    private int floorNumber = GameProperties.FloorNumber;
    private static Quaternion quaternion;

    private void Start()
    {
        var rows = dataTable.Select(row => row.ToDataTableRow()).ToArray();
        var dataTableFs = new DataTable.DataTable<GameObject>(rows);
        var room = new Room<GameObject>(roomLength, roomWidth, floorNumber, dataTableFs);
        var tupleFunc =
            FuncConvert.ToFSharpFunc<Tuple<DataTable.DataTableRow<GameObject>, DataTable.ObjectVariant<GameObject>, int, int>>(
                    t => PlaceObject(t.Item1, t.Item2, t.Item3, t.Item4));
        var func = FuncConvert.FuncFromTupled(tupleFunc);

        room.GenerateInterior(maximumAmountOfObjects, func);

    }
    private void PlaceObject(DataTable.DataTableRow<GameObject> objectToPlace,
        DataTable.ObjectVariant<GameObject> objectToPlaceVariant, int rowIndex, int columnIndex)
    {
        var prefab = objectToPlaceVariant.Variant;
        var placementRule = objectToPlace.PlacementRule;
        switch (placementRule.ToString())
        {
            case "Node None":
                quaternion = Quaternion.Euler(0, 180, 0);
                break;
            case "Node AgainstTheBottomWall":
                quaternion = Quaternion.Euler(0, 0, 0);
                break;
            case "Node AgainstTheTopWall":
                quaternion = Quaternion.Euler(0, 180, 0);
                break;
            case "Node AgainstTheRightWall":
                quaternion = Quaternion.Euler(0, 270, 0);
                break;
            case "Node AgainstTheLeftWall":
                quaternion = Quaternion.Euler(0, 90, 0);
                break;
            case "Node InTheCorner":
                quaternion = Quaternion.Euler(0, 180, 0);
                break;
            case "Leaf LeftTo":
                quaternion = Quaternion.Euler(0, 270, 0);
                break;
            case "Leaf RightTo":
                quaternion = Quaternion.Euler(0, 90, 0);
                break;
            case "Leaf Behind":
                quaternion = Quaternion.Euler(0, 0, 0);
                break;
            case "Leaf InFrontOf":
                quaternion = Quaternion.Euler(0, 180, 0);
                break;
            case "Leaf Anywhere":
                quaternion = Quaternion.Euler(0, 0, 0);
                break;
            default:
                throw new InvalidOperationException($"Unknown placementRule value: {placementRule}");
        }

        var delta = new Vector3(rowIndex + transform.position.x, transform.position.y, columnIndex + transform.position.z);
        Instantiate(prefab, delta, quaternion, transform);
    }

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

        private DataTable.Rule ParseRule()
        {
            switch (placementRule)
            {
                case PlacementRuleWrapper.WithoutRule:
                    return DataTable.Rule.NewNode(DataTable.NodePlacementRule.None);

                case PlacementRuleWrapper.AgainstTheBottomWall:
                    return DataTable.Rule.NewNode(DataTable.NodePlacementRule.AgainstTheBottomWall);

                case PlacementRuleWrapper.AgainstTheTopWall:
                    return DataTable.Rule.NewNode(DataTable.NodePlacementRule.AgainstTheTopWall);

                case PlacementRuleWrapper.AgainstTheRightWall:
                    return DataTable.Rule.NewNode(DataTable.NodePlacementRule.AgainstTheRightWall);

                case PlacementRuleWrapper.AgainstTheLeftWall:
                    return DataTable.Rule.NewNode(DataTable.NodePlacementRule.AgainstTheLeftWall);

                case PlacementRuleWrapper.InTheCorner:
                    return DataTable.Rule.NewNode(DataTable.NodePlacementRule.InTheCorner);

                case PlacementRuleWrapper.LeftToParent:
                    return DataTable.Rule.NewLeaf(DataTable.LeafPlacementRule.LeftTo);

                case PlacementRuleWrapper.RightToParent:
                    return DataTable.Rule.NewLeaf(DataTable.LeafPlacementRule.RightTo);

                case PlacementRuleWrapper.BehindParent:
                    return DataTable.Rule.NewLeaf(DataTable.LeafPlacementRule.Behind);

                case PlacementRuleWrapper.InFrontOfParent:
                    return DataTable.Rule.NewLeaf(DataTable.LeafPlacementRule.InFrontOf);

                case PlacementRuleWrapper.AnywhereNearParent:
                    return DataTable.Rule.NewLeaf(DataTable.LeafPlacementRule.Anywhere);
                default:
                    throw new InvalidOperationException($"Unknown placementRule value: {placementRule}");
            }
        }

        public DataTable.DataTableRow<GameObject> ToDataTableRow()
        {
            var variantsArray = variants.Select(variant => variant.ToObjectVariant()).ToArray();
            var leafsDataTable = leafs.Select(row => row.ToDataTableRow()).ToArray();
            var dataTableRow = new DataTable.DataTableRow<GameObject>(name, variantsArray, ParseRule(), leafsDataTable);
            return dataTableRow;
        }

    }

    [Serializable]
    public class ObjectVariantWrapper
    {
        [SerializeField] private GameObject variant;
        [SerializeField] private int freeCellsOnRight;
        [SerializeField] private int freeCellsOnLeft;
        [SerializeField] private int freeCellsOnTop;
        [SerializeField] private int freeCellsOnBottom;

        public DataTable.ObjectVariant<GameObject> ToObjectVariant()
        {
            var objectVariant = new DataTable.ObjectVariant<GameObject>(variant, freeCellsOnLeft, freeCellsOnRight, freeCellsOnTop, freeCellsOnBottom);
            return objectVariant;
        }

    }
}

