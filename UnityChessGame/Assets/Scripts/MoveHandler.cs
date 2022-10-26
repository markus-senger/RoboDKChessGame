using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHandler : MonoBehaviour
{
    public bool isMoving { get; set; }
    public GameObject movingFigure { get; set; }

    public List<GameObject> whiteGraveyard { get; private set; }

    public List<GameObject> blackGraveyard { get; private set; }

    private void Awake()
    {
        whiteGraveyard = new List<GameObject>();
        blackGraveyard = new List<GameObject>();
    }
}
