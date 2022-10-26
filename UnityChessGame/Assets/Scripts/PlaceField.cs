using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlaceField : MonoBehaviour
{
    [SerializeField]
    private GameObject sceneHandler;

    public GameObject currentFigureOnField { get; set; }

    private Color orgColor;
    private MoveHandler moveHandler;
    private TCPIPClient tcpIpClient;

    private void Awake()
    {
        orgColor = GetComponent<Renderer>().material.color;
        moveHandler = sceneHandler.GetComponent<MoveHandler>();
        tcpIpClient = sceneHandler.GetComponent<TCPIPClient>();
    }

    private async void OnMouseOver()
    {
        if(moveHandler.isMoving && tcpIpClient.serverReady)
        {
            GetComponent<Renderer>().material.color = Color.yellow;
            if (Input.GetMouseButtonDown(0))
            {
                if (currentFigureOnField != null)
                {
                    if (currentFigureOnField.name.Contains("w"))
                    {
                        await MoveToGraveyard(false);
                        currentFigureOnField.GetComponentInChildren<MoveFigure>().currentField = null;
                    }
                    else if (currentFigureOnField.name.Contains("s"))
                    {
                        await MoveToGraveyard(true);
                        currentFigureOnField.GetComponentInChildren<MoveFigure>().currentField = null;
                    }
                }

                if (moveHandler.blackGraveyard.Contains(moveHandler.movingFigure.transform.parent.gameObject))
                {
                    int idx = moveHandler.blackGraveyard.FindIndex(f => f == moveHandler.movingFigure.transform.parent.gameObject);
                    moveHandler.blackGraveyard[idx] = null;
                }
                else if (moveHandler.whiteGraveyard.Contains(moveHandler.movingFigure.transform.parent.gameObject))
                {
                    int idx = moveHandler.whiteGraveyard.FindIndex(f => f == moveHandler.movingFigure.transform.parent.gameObject);
                    moveHandler.whiteGraveyard[idx] = null;
                }

                Vector3 pos = moveHandler.movingFigure.transform.localPosition;
                Vector3 posParent = moveHandler.movingFigure.transform.parent.localPosition;

                moveHandler.movingFigure.transform.position = gameObject.transform.position;
                if(moveHandler.movingFigure.GetComponent<MoveFigure>().currentField != null)
                    moveHandler.movingFigure.GetComponent<MoveFigure>().currentField.GetComponent<PlaceField>().currentFigureOnField = null;

                moveHandler.movingFigure.GetComponent<MoveFigure>().currentField = gameObject;
                moveHandler.movingFigure.GetComponent<MoveFigure>().currentField.GetComponent<PlaceField>().currentFigureOnField = moveHandler.movingFigure.transform.parent.gameObject;

                await sendMoveCommandToRoboDK(
                    pos,
                    posParent,
                    moveHandler.movingFigure.transform.localPosition,
                    moveHandler.movingFigure.transform.parent.localPosition
                );

                moveHandler.isMoving = false;
                moveHandler.movingFigure = null;
            }
        }
    }

    private void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = orgColor;
    }

    private async Task MoveToGraveyard(bool isBlack)
    {
        if(isBlack)
        {
            int idx = moveHandler.blackGraveyard.FindIndex(f => f == null);
            if (idx != -1) 
            {
                moveHandler.blackGraveyard[idx] = currentFigureOnField;
                idx++;
            }
            else
            {
                moveHandler.blackGraveyard.Add(currentFigureOnField);
                idx = moveHandler.blackGraveyard.Count;
            }

            Vector3 pos = currentFigureOnField.transform.GetComponentInChildren<MeshCollider>().gameObject.transform.localPosition;
            Vector3 posParent = currentFigureOnField.transform.localPosition;

            currentFigureOnField.transform.GetComponentInChildren<MeshCollider>().gameObject.transform.position = new Vector3(500 - 60 * idx, 20, -450);

            await sendMoveCommandToRoboDK(
                    pos,
                    posParent,
                    currentFigureOnField.transform.GetComponentInChildren<MeshCollider>().gameObject.transform.localPosition,
                    currentFigureOnField.transform.localPosition
                );
        }
        else
        {
            int idx = moveHandler.whiteGraveyard.FindIndex(f => f == null);
            if (idx != -1)
            {
                moveHandler.whiteGraveyard[idx] = currentFigureOnField;
                idx++;
            }
            else
            {
                moveHandler.whiteGraveyard.Add(currentFigureOnField);
                idx = moveHandler.whiteGraveyard.Count;
            }

            Vector3 pos = currentFigureOnField.transform.GetComponentInChildren<MeshCollider>().gameObject.transform.localPosition;
            Vector3 posParent = currentFigureOnField.transform.localPosition;

            currentFigureOnField.transform.GetComponentInChildren<MeshCollider>().gameObject.transform.position = new Vector3(500 - 60 * idx, 20, 450);

            await sendMoveCommandToRoboDK(
                    pos,
                    posParent,
                    currentFigureOnField.transform.GetComponentInChildren<MeshCollider>().gameObject.transform.localPosition,
                    currentFigureOnField.transform.localPosition
                );
        }
    }

    private async Task sendMoveCommandToRoboDK(Vector3 posStep1, Vector3 posParentStep1, Vector3 posStep2, Vector3 posParentStep2)
    {
        try
        {
            await Task.Run(() =>
            {
                tcpIpClient.sendCommand(
                    posStep1.x + posParentStep1.x,
                    (posStep1.z + posParentStep1.z) * -1,
                    "AttachFigure");
            });

            await Task.Run(() =>
            {
                tcpIpClient.sendCommand(
                    posStep2.x + posParentStep2.x,
                    (posStep2.z + posParentStep2.z) * -1,
                    "DetachFigure");
            });
        }
        catch
        {
            tcpIpClient.ConnectionErrorHandler();
        }
    }
}
