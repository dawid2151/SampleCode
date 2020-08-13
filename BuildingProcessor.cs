using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using EdTools.Unity;

public class BuildingProcessor : MonoBehaviour
{
    [SerializeField] private LayerMask _placingObjectLayer;
    [SerializeField] private GameObject _placer;                //In game GameObject that represents that semi-transparent "building cursor"
    [SerializeField] private GameObject _placerMeshHolder;
    [SerializeField] private MeshFilter _placerMeshFilter;
    [SerializeField] private MeshRenderer _placerMeshRenderer;
    [SerializeField] private Material _canPlace_MAT;
    [SerializeField] private Material _canNotPlace_MAT;

    private BuildingType _selectedBuildingType;
    private Dictionary<BuildingSize, Vector3> _buildingScales;
    private bool _isPlacing = false;

    private void Start()
    {
        _buildingScales = new Dictionary<BuildingSize, Vector3>
        {
            {BuildingSize.Small, new Vector3(6f,1f,6f) },
            {BuildingSize.Medium, new Vector3(18f,1f, 18f) },
            {BuildingSize.Big, new Vector3(30f,1f, 30f) }
        };

    }
    private void Update()
    {
        if (!_isPlacing)
            return;

        Vector3 currentBuildPosition;
        RaycastHit hit;     

        if (HitValidPlacingGround(out hit))
        {
            SnapBuildingPositionToGrid(hit, out currentBuildPosition);

            _placer.transform.position = currentBuildPosition;

            if (CheckBuildRequirements(currentBuildPosition))
                OnCanBuild(currentBuildPosition);
            else
                OnCanNotBuild(currentBuildPosition);

        }
    }
    public void StartPlacing(BuildingType buildingType)
    {
        _isPlacing = true;
        _placer.SetActive(true);
        SetBuildingType(buildingType);
    }
    public void StopPlacing()
    {
        _isPlacing = false;
        _placer.SetActive(false);
    }


    private bool CheckBuildRequirements(Vector3 buildPosition)
    {
        if (PlacerController.IsColliding)
            return false;
        if (!BuildingManager.BuildingsData[_selectedBuildingType].BuildingPrefab.GetComponent<Building>().BuildConditionMet())
            return false;

        return true;
    }
    private void OnCanBuild(Vector3 placePosition)
    {
        _placerMeshRenderer.material = _canPlace_MAT;
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (CanAffordBuilding())
                PlaceSelectedBuilding(placePosition);
        }
    }
    private void OnCanNotBuild(Vector3 placePosition)
    {
        _placerMeshRenderer.material = _canNotPlace_MAT;
    }
    private void OnCanNotAfford(ResourceType resourceType)
    {
        Debug.Log("Not enough " + resourceType.ToString());
    }
    private void SnapBuildingPositionToGrid(RaycastHit hit, out Vector3 placePosition)
    {
        placePosition = hit.collider.transform.position.With(y: 0.1f);
    }
    private void PlaceSelectedBuilding(Vector3 placePosition)
    {
        GameObject tempB = GameManager.Instantiate(BuildingManager.BuildingsData[_selectedBuildingType].BuildingPrefab, placePosition, Quaternion.identity);
        tempB.GetComponent<Building>()?.OnPlace();

        for (int i = 0; i < BuildingManager.BuildingsData[_selectedBuildingType].NeededResources.Length; i++)
        {
            GameManager.MainResourceHolder.AddResource((ResourceType)i, -BuildingManager.BuildingsData[_selectedBuildingType].NeededResources[i]);
        }
    }
    private void SetBuildingType(BuildingType buildingType)
    {
        _selectedBuildingType = buildingType;

        _placerMeshFilter.mesh = BuildingManager.BuildingsData[_selectedBuildingType].BuildingMesh;
        _placer.transform.localScale = _buildingScales[BuildingManager.BuildingsData[_selectedBuildingType].BuildingSizeType];
        _placerMeshFilter.transform.localScale = new Vector3(
                1f / _buildingScales[BuildingManager.BuildingsData[_selectedBuildingType].BuildingSizeType].x, 1f,
                1f / _buildingScales[BuildingManager.BuildingsData[_selectedBuildingType].BuildingSizeType].z);
    }
    private bool HitValidPlacingGround(out RaycastHit hit)
    {
        Ray ray = GameManager.MyCamera.ScreenPointToRay(Input.mousePosition);

        return Physics.Raycast(ray, out hit, Mathf.Infinity, _placingObjectLayer);
    }
    private bool CanAffordBuilding()
    {
        bool outcome = true;
        for (int i = 0; i < BuildingManager.BuildingsData[_selectedBuildingType].NeededResources.Length; i++)
        {
            if (BuildingManager.BuildingsData[_selectedBuildingType].NeededResources[i] > GameManager.MainResourceHolder.PossesedResources[(ResourceType)i])
            {
                OnCanNotAfford((ResourceType)i);
                outcome = false;
            }
        }

        return outcome;
    }
}
