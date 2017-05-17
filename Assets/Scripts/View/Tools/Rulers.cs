﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementTemplates {

    private GameManagerScript Game;

	private Vector3 savedRulerPosition;
	private Vector3 savedRulerRotation;
    private List<Vector3> rulerCenterPoints = new List<Vector3>();

    private Transform Templates;
    public Transform CurrentTemplate;

    public void Initialize()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        Game.Actions.OnCheckCanPerformAttack += CallShowRange;
        Templates = Game.Board.RulersHolder.transform;
    }

    public void AddRulerCenterPoint(Vector3 point)
    {
        rulerCenterPoints.Add(point);
    }

    public void ResetRuler(Ship.GenericShip ship)
    {
        rulerCenterPoints = new List<Vector3>();
        HideLastMovementRuler();
    }

    public Vector3 FindNearestRulerCenterPoint(Vector3 pointShipStand)
    {
        Vector3 result = Vector3.zero;
        float minDistance = float.MaxValue;
        foreach (Vector3 rulerCenterPoint in rulerCenterPoints)
        {
            float currentDistance = Vector3.Distance(rulerCenterPoint, pointShipStand);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                result = rulerCenterPoint;
            }
        }

        return result;
    }

    public void ApplyMovementRuler(Ship.GenericShip thisShip) {

        if (Game.Movement.CurrentMovementData.Speed != 0)
        {
            CurrentTemplate = GetMovementRuler();
            savedRulerPosition = CurrentTemplate.position;
            savedRulerRotation = CurrentTemplate.eulerAngles;

            CurrentTemplate.position = thisShip.Model.GetPosition();
            CurrentTemplate.eulerAngles = thisShip.Model.GetAngles() + new Vector3(0f, 90f, 0f);
            if (Game.Movement.CurrentMovementData.MovementDirection == ManeuverDirection.Left)
            {
                CurrentTemplate.eulerAngles = CurrentTemplate.eulerAngles + new Vector3(180f, 0f, 0f);
            }
        }
        
	}

    private Transform GetMovementRuler()
    {
        Transform result = null;
        switch (Game.Movement.CurrentMovementData.MovementBearing)
        {
            case ManeuverBearing.Straight:
                return Templates.Find("straight" + Game.Movement.CurrentMovementData.Speed);
            case ManeuverBearing.Bank:
                return Templates.Find("bank" + Game.Movement.CurrentMovementData.Speed);
            case ManeuverBearing.Turn:
                return Templates.Find("turn" + Game.Movement.CurrentMovementData.Speed);
            case ManeuverBearing.KoiogranTurn:
                return Templates.Find("straight" + Game.Movement.CurrentMovementData.Speed);
        }
        return result;
    }

    private void HideLastMovementRuler(){
        CurrentTemplate.position = savedRulerPosition;
		CurrentTemplate.eulerAngles = savedRulerRotation;
	}

    public void CallShowRange(ref bool result, Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        ShowRange(thisShip, anotherShip);
    }

    public void ShowRange(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        Vector3 vectorToTarget = thisShip.Model.GetClosestEdgesTo(anotherShip)["another"] - thisShip.Model.GetClosestEdgesTo(anotherShip)["this"];
        Templates.Find("RangeRuler").position = thisShip.Model.GetClosestEdgesTo(anotherShip)["this"];
        Templates.Find("RangeRuler").rotation = Quaternion.LookRotation(vectorToTarget);
    }

    public void CallReturnRangeRuler(Ship.GenericShip thisShip)
    {
        ReturnRangeRuler();
    }

    public void ReturnRangeRuler()
    {
        Templates.Find("RangeRuler").transform.position = new Vector3(9.5f, 0f, 2.2f);
        Templates.Find("RangeRuler").transform.eulerAngles = new Vector3(0, -90, 0);
    }

}
