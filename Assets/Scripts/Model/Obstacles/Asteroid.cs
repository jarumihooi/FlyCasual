﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ship;
using UnityEngine;

namespace Obstacles
{
    public class Asteroid: GenericObstacle
    {
        public Asteroid(string name) : base(name)
        {

        }

        public override string GetTypeName => "Asteroid";

        public override void OnHit(GenericShip ship)
        {
            // no action
            // roll die
        }

        public override void OnLanded(GenericShip ship)
        {
            // cannot shoot
        }

        public override void OnShotObstructed(GenericShip attacker, GenericShip defender)
        {
            // +1 die
        }
    }
}
