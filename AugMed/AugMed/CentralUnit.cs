using System;
using System.Collections.Generic;
using System.Collections;
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
    public class CentralUnit
    {
        List<MarkerNode> patientMarkers;
        //List<MarkerNode> equipmentMarkers;
        Hashtable equipmentMarkers;
        MarkerModule markerM;
        PatientModule patient;

        public CentralUnit()
        {
            patientMarkers = new List<MarkerNode>();
            //equipmentMarkers = new List<MarkerNode>();
            equipmentMarkers = new Hashtable();
            markerM = new MarkerModule();
            patient = new PatientModule();
        }

        public String getPatientName(Object markerID)
        {
            return patient.getPatientName(markerID);
        }

        public TransformNode getPatientNameNode(Object markerID)
        {
            return patient.getPatientNameNode(markerID);
        }


        /*
        public List<Node> CreateChildren(Object markerID)
        {
            //take patientID and get equipment associated with the pt
            //get settings associated that need to be rendered
            //use equipID from patient to get location
            //get text
            //put text together dynamically
            //get the location
            //put it on transformNode
            //add transformNode to list of nodes ans make key the equipID
            //return list of nodes
        }
        */

        public void RemoveAllChildren(Object markerID)
        {
            //get the equipment associated with the pt with id = markerID
            //getEquipment(patient markerID) -> adds equipment
            //remove their children
        }

        public List<MarkerNode> getAllPatientMarkers(Scene scene)
        {
            MarkerNode temp = markerM.getPtMarkerNode(scene);
            patientMarkers.Add(temp);
            return patientMarkers;
        }

        //public List<MarkerNode> getAllEquipmentMarkers(Scene scene)
        public Hashtable getAllEquipmentMarkers(Scene scene)
        {
            MarkerNode marker = markerM.getEquipMarkerNode(scene);
            equipmentMarkers.Add(marker.MarkerID, marker);
            return equipmentMarkers;
        }

        public Hashtable getInformation(Object PtMarkerID)
        {
            Hashtable hsh = new Hashtable();
            List<Object> equip = patient.getListEquip(PtMarkerID);
            foreach (Object eqID in equip)
            {
                List<TransformNode> nodes = getListTransformNodes(PtMarkerID, eqID);
                hsh.Add(eqID, nodes);
            }
            return hsh;
        }

        public List<TransformNode> getListTransformNodes(Object PtID, Object EqID)
        {
            //create Box
            GeometryNode boxNode = new GeometryNode("Box");
            boxNode.Model = new Box(6);
            boxNode.Model.CastShadows = true;
            boxNode.Model.ReceiveShadows = true;
            Material boxMat = new Material();
            boxMat.Diffuse = Color.Blue.ToVector4();
            boxMat.Specular = Color.White.ToVector4();
            boxMat.SpecularPower = 20;
            boxNode.Material = boxMat;
            TransformNode boxTrans = new TransformNode();
            boxTrans.Translation = new Vector3(-35, -18, 8);
            boxTrans.AddChild(boxNode);

            //Create cylinder
            GeometryNode cylinderNode = new GeometryNode("Cylinder");
            cylinderNode.Model = new Cylinder(3.5f, 3.5f, 10, 20);
            cylinderNode.Model.CastShadows = true;
            cylinderNode.Model.ReceiveShadows = true;
            Material cylinderMat = new Material();
            cylinderMat.Diffuse = Color.Green.ToVector4();
            cylinderMat.Specular = Color.White.ToVector4();
            cylinderMat.SpecularPower = 20;
            cylinderNode.Material = cylinderMat;
            TransformNode cylinderTrans = new TransformNode();
            cylinderTrans.Translation = new Vector3(35, -18, 8);
            cylinderTrans.AddChild(cylinderNode);

            List<TransformNode> nodes = new List<TransformNode>();
            nodes.Add(boxTrans);
            nodes.Add(cylinderTrans);
            return nodes;
        }
    }
}
