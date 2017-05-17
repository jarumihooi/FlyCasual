﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPositionManager : MonoBehaviour
{

    private GameManagerScript Game;
    public bool inReposition;

    public GameObject prefabShipStand;
    private GameObject OriginalShipStand;

    // Use this for initialization
    void Start()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Game == null) Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        if (Game.Selection == null) Game.Selection = GameObject.Find("GameManager").GetComponent<ShipSelectionManagerScript>();

        if (Game.Selection.ThisShip != null)
        {
            StartDrag();
        }

        if (inReposition)
        {
            PerformDrag();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            //StopDrag();
        }

    }

    public void StartDrag()
    {
        if (Game.Phases.CurrentPhase.GetType() == typeof(Phases.SetupPhase)) {
            Game.Roster.SetRaycastTargets(false);
            inReposition = true;
        }
    }

    public void StartBarrelRoll()
    {
        if (Game.Phases.CurrentSubPhase.GetType() == typeof(SubPhases.BarrelRollSubPhase))
        {
            OriginalShipStand = MonoBehaviour.Instantiate(Game.Position.prefabShipStand, Game.Selection.ThisShip.Model.GetPosition(), Game.Selection.ThisShip.Model.GetRotation(), Game.Board.Board.transform);
            Game.Roster.SetRaycastTargets(false);
            inReposition = true;
        }
    }

    private void PerformDrag()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Game.Selection.ThisShip.Model.SetPosition(new Vector3(hit.point.x, 0.03f, hit.point.z));
        }

        if (Game.Phases.CurrentSubPhase.GetType() == typeof(SubPhases.BarrelRollSubPhase))
        {
            //Write Relative position
            Vector3 newPosition = OriginalShipStand.transform.InverseTransformPoint(Game.Selection.ThisShip.Model.GetPosition());
            Vector3 fixedPositionRel = newPosition;

            Transform currentHelper = OriginalShipStand.transform.Find("Right");

            if (newPosition.z > 0.5f)
            {
                fixedPositionRel = new Vector3(fixedPositionRel.x, fixedPositionRel.y, 0.5f);
            }

            if (newPosition.z < -0.5f)
            {
                fixedPositionRel = new Vector3(fixedPositionRel.x, fixedPositionRel.y, -0.5f);
            }

            if (newPosition.x > 0f) {
                fixedPositionRel = new Vector3(2, fixedPositionRel.y, fixedPositionRel.z);

                currentHelper = OriginalShipStand.transform.Find("Right");
                currentHelper.gameObject.SetActive(true);
                OriginalShipStand.transform.Find("Left").gameObject.SetActive(false);
            }

            if (newPosition.x < 0f) {
                fixedPositionRel = new Vector3(-2, fixedPositionRel.y, fixedPositionRel.z);

                currentHelper = OriginalShipStand.transform.Find("Left");
                currentHelper.gameObject.SetActive(true);
                OriginalShipStand.transform.Find("Right").gameObject.SetActive(false);
            }

            Vector3 helperPositionRel = OriginalShipStand.transform.InverseTransformPoint(currentHelper.position);
            if (fixedPositionRel.z-0.25 < helperPositionRel.z)
            {
                helperPositionRel = new Vector3(helperPositionRel.x, helperPositionRel.y, fixedPositionRel.z-0.25f);
                Vector3 helperPositionAbs = OriginalShipStand.transform.TransformPoint(helperPositionRel);
                currentHelper.position = helperPositionAbs;
            }
            if (fixedPositionRel.z-0.75 > helperPositionRel.z)
            {
                helperPositionRel = new Vector3(helperPositionRel.x, helperPositionRel.y, fixedPositionRel.z - 0.75f);
                Vector3 helperPositionAbs = OriginalShipStand.transform.TransformPoint(helperPositionRel);
                currentHelper.position = helperPositionAbs;
            }

            Vector3 fixedPositionAbs = OriginalShipStand.transform.TransformPoint(fixedPositionRel);
            Game.Selection.ThisShip.Model.SetPosition(fixedPositionAbs);
        }
    }

    //TODO: Good target to move into subphase class
    public bool TryConfirmPosition(Ship.GenericShip ship)
    {
        bool result = true;

        //TODO:
        //Cannot leave board
        //Obstacles

        if (Game.Phases.CurrentSubPhase.GetType() == typeof(SubPhases.SetupSubPhase))
        {
            GameObject startingZone = (Game.Phases.CurrentSubPhase.RequiredPlayer == Players.PlayerNo.Player1) ? Game.Board.StartingZone1 : Game.Board.StartingZone2;
            if (!ship.Model.IsInside(startingZone.transform))
            {
                Game.UI.ShowError("Place ship into highlighted area");
                result = false;
            }
        }

        if (Game.Phases.CurrentSubPhase.GetType() == typeof(SubPhases.BarrelRollSubPhase))
        {
            Destroy(OriginalShipStand);
            result = true;
        } 

        if (Game.Movement.CollidedWith != null)
        {
            Game.UI.ShowError("This ship shouldn't collide with another ships");
            result = false;
        }

        if (result) StopDrag();
        return result;
    }

    private void StopDrag()
    {
        Game.Selection.DeselectThisShip();
        Game.Roster.SetRaycastTargets(true);
        inReposition = false;
        //Should I change subphase immediately?
        Game.Phases.CurrentSubPhase.NextSubPhase();
    }



}
