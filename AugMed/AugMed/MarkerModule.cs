using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Device.Util;
using GoblinXNA.Physics;
using GoblinXNA.Helpers;

namespace AugMed
{
    public class MarkerModule
    {
        public MarkerModule()
        {
        }

        public String getMarkerNode()
        {
            return "Got Marker";
        }

        public MarkerNode getPtMarkerNode(Scene scene)
        {
            int[] ids = new int[1];
            ids[0] = 0;
            //return new MarkerNode(scene.MarkerTracker, "Patient.txt", ids); //ids[0]);
            //return new MarkerNode(scene.MarkerTracker, "Toolbar.txt", ids); //ids[0]);  
            return new MarkerNode(scene.MarkerTracker, "PatientMarkerConfig.txt", ids);
            //return new MarkerNode(scene.MarkerTracker, ids[0], "PatientMarkerConfig.txt"); 
        }

        public MarkerNode getEquipMarkerNode(Scene scene)
        {
            int[] ids = new int[1];
            ids[0] = 1;
            //return new MarkerNode(scene.MarkerTracker, "Equipment.txt", ids); //ids[1]);
            return new MarkerNode(scene.MarkerTracker, "EquipementMarkerConfig.txt", ids); //ids[1]);
        }

        public Object getMarkerID()
        {
            return null;
        }
    }
}
