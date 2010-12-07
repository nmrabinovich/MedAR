using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    public class PatientModule
    {
        public PatientModule()
        {
        }

        public TransformNode getPatientNameNode(Object markerID)
        {
            if (markerID.Equals("PatientMarkerConfig.txt"))
            {
                GeometryNode sphereNode = new GeometryNode("Sphere");
                sphereNode.Model = new Sphere(3.5f, 20, 20);
                sphereNode.Model.CastShadows = true;
                sphereNode.Model.ReceiveShadows = true;

                Material sphereMat = new Material();
                sphereMat.Diffuse = Color.Red.ToVector4();
                sphereMat.Specular = Color.White.ToVector4();
                sphereMat.SpecularPower = 20;

                sphereNode.Material = sphereMat;
                //sphereNode.Physics.Interactable = false;

                TransformNode sphereTrans = new TransformNode();
                sphereTrans.Translation = new Vector3(0, 0, 5);
                sphereTrans.AddChild(sphereNode);
                return sphereTrans;
            }
            return null;
        }

        public List<Object> getListEquip(Object PtID)
        {
            List<Object> equip = new List<object>();
            equip.Add("EquipementMarkerConfig.txt");
            //equip.Add("Test.txt");
            return equip;
        }

        public String getPatientName(Object markerID)
        {
            if (markerID.Equals("PatientMarkerConfig.txt"))
                return "Smith, John";
            else if (markerID.Equals(2))
                return "Doe, Jane";
            return "No Patient";
        }
    }
}
