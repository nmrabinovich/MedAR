using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

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
using GoblinXNA.UI.UI2D;

namespace AugMed
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class AnnotationsModule : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont sampleFont;
        MarkerNode EquipmentMarkerNode, PatientMarkerNode;
        Scene scene;
        MarkerModule MM;
        List<MarkerNode> patients;
        Hashtable equipment;
        Hashtable transformationHsh;
        CentralUnit central;
        //String marker;
        // set this to false if you are going to use a webcam
        bool useStaticImage = false;
        // -1 if inactive, 1 if active
        int found = -1;
        int dropOut = 0;
        int maxDropOut = 100;

        public AnnotationsModule()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Initialize the GoblinXNA framework
            State.InitGoblin(graphics, Content, "");

            MM = new MarkerModule();
            central = new CentralUnit();
            // Initialize the scene graph
            scene = new Scene(this);

            // Use the newton physics engine to perform collision detection
            scene.PhysicsEngine = new NewtonPhysics();

            // Set up optical marker tracking
            // Note that we don't create our own camera when we use optical marker
            // tracking. It'll be created automatically
            SetupMarkerTracking();

            // Set up the lights used in the scene
            CreateLights();

            //adds markers to the scene to be detected and IDed by goggles
            patients = central.getAllPatientMarkers(scene);
            equipment = central.getAllEquipmentMarkers(scene);
            //equipment = new Hashtable();
            AddMarkersToScene();
            // Create 3D objects
            //CreateObjects();

            // Create the ground that represents the physical ground marker array
            //CreateGround();

            // Use per pixel lighting for better quality (If you using non NVidia graphics card,
            // setting this to true may reduce the performance significantly)
            scene.PreferPerPixelLighting = true;

            // Enable shadow mapping
            // NOTE: In order to use shadow mapping, you will need to add 'PostScreenShadowBlur.fx'
            // and 'ShadowMap.fx' shader files as well as 'ShadowDistanceFadeoutMap.dds' texture file
            // to your 'Content' directory
            scene.EnableShadowMapping = false ;

            // Show Frames-Per-Second on the screen for debugging
            State.ShowFPS = false;

            base.Initialize();
        }


        private void CreateLights()
        {
            // Create a directional light source
            LightSource lightSource = new LightSource();
            lightSource.Direction = new Vector3(1, -1, -1);
            lightSource.Diffuse = Color.White.ToVector4();
            lightSource.Specular = new Vector4(0.6f, 0.6f, 0.6f, 1);

            // Create a light node to hold the light source
            LightNode lightNode = new LightNode();
            lightNode.LightSources.Add(lightSource);

            scene.RootNode.AddChild(lightNode);
        }

        private void SetupMarkerTracking()
        {
            // Create our video capture device that uses DirectShow library. Note that 
            // the combinations of resolution and frame rate that are allowed depend on 
            // the particular video capture device. Thus, setting incorrect resolution 
            // and frame rate values may cause exceptions or simply be ignored, depending 
            // on the device driver.  The values set here will work for a Microsoft VX 6000, 
            // and many other webcams.
            IVideoCapture captureDevice = null;

            if (useStaticImage)
            {
                captureDevice = new NullCapture();
                captureDevice.InitVideoCapture(0, FrameRate._30Hz, Resolution._800x600,
                    ImageFormat.R8G8B8_24, false);

                ((NullCapture)captureDevice).StaticImageFile = "testImage800x600.jpg";
            }
            else
            {
                captureDevice = new DirectShowCapture2();
                captureDevice.InitVideoCapture(0, FrameRate._30Hz, Resolution._640x480, ImageFormat.R8G8B8_24, false);
            }
            // Add this video capture device to the scene so that it can be used for
            // the marker tracker
            scene.AddVideoCaptureDevice(captureDevice);

            IMarkerTracker tracker = null;


            // Create an optical marker tracker that uses ALVAR library
            tracker = new ALVARMarkerTracker();
            ((ALVARMarkerTracker)tracker).MaxMarkerError = 0.02f;
            tracker.InitTracker(captureDevice.Width, captureDevice.Height, "calib.xml", 9.0);

            // Set the marker tracker to use for our scene
            scene.MarkerTracker = tracker;

            // Display the camera image in the background. Note that this parameter should
            // be set after adding at least one video capture device to the Scene class.
            scene.ShowCameraImage = true;
        }

        /*
        private void CreateGround()
        {
            GeometryNode groundNode = new GeometryNode("Ground");
            groundNode.Model = new Box(95, 59, 0.1f);

            // Set this ground model to act as an occluder so that it appears transparent
            groundNode.IsOccluder = true;

            // Make the ground model to receive shadow casted by other objects with
            // CastShadows set to true
            groundNode.Model.ReceiveShadows = true;

            Material groundMaterial = new Material();
            groundMaterial.Diffuse = Color.Gray.ToVector4();
            groundMaterial.Specular = Color.White.ToVector4();
            groundMaterial.SpecularPower = 20;

            groundNode.Material = groundMaterial;

            //groundMarkerNode.AddChild(groundNode);
        }
        */

        private void AddMarkersToScene()
        {
            //PatientMarkerNode = (MarkerNode) MM.getPtMarkerNode(scene);
            //EquipmentMarkerNode = (MarkerNode) MM.getEquipMarkerNode(scene);
            //scene.RootNode.AddChild(PatientMarkerNode);
            //scene.RootNode.AddChild(EquipmentMarkerNode);
             
            foreach (MarkerNode marker in patients)
            {
                //marker.Enabled = false;
                scene.RootNode.AddChild(marker);
            }
            foreach (Object eqID in equipment.Keys)
            {
                scene.RootNode.AddChild((MarkerNode)equipment[eqID]);
            }
            //marker = MM.getMarkerNode();
            //Console.WriteLine("Marker is: " + marker); 
        }

        private void CreateObjects()
        {
            //get marker ID to pass to patientModule as primary key
            //Object ID = PatientMarkerNode.MarkerID();

            
            // Create a sphere geometry
            /*
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

                PatientMarkerNode.AddChild(sphereTrans);
                sphereTrans.AddChild(sphereNode);
            */

            // Create a box geometry
            
                GeometryNode boxNode = new GeometryNode("Box");
                boxNode.Model = new Box(6);
                boxNode.Model.CastShadows = true;
                boxNode.Model.ReceiveShadows = true;

                Material boxMat = new Material();
                boxMat.Diffuse = Color.Blue.ToVector4();
                boxMat.Specular = Color.White.ToVector4();
                boxMat.SpecularPower = 20;

                boxNode.Material = boxMat;
                //boxNode.Physics.Interactable = true;

                TransformNode boxTrans = new TransformNode();
                boxTrans.Translation = new Vector3(-35, -18, 8);

                //EquipmentMarkerNode.AddChild(boxTrans);
                boxTrans.AddChild(boxNode);
            
            //get patient controller and setting; store it
            //    if patient has information, obtain it and get equipment information
            //get equipment controller, location, and text; store it
            //create 2D text of setting, location, & text; and add to markerNode
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sampleFont = Content.Load<SpriteFont>("Sample");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void AddInformation(Hashtable hsh)
        {
            foreach (Object key in hsh.Keys)
            {
                foreach (TransformNode node in (List<TransformNode>)hsh[key])
                {
                    ((MarkerNode)equipment[key]).AddChild(node);
                }
            }
        }

        private void RemoveInformation()
        {
            foreach (Object key in transformationHsh.Keys)
            {
                ((MarkerNode)equipment[key]).RemoveChildren();
            }
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            //PatientMarkerNode = null;
            /*if patient marker is found*/
            int i;
            int current = 0;
            if (found == -1)
            {
                for (i = 0; i < patients.Count; i++)
                {
                    if (patients[i].MarkerFound == true)
                    {
                        PatientMarkerNode = patients[i];
                        current = i;
                        i = patients.Count;
                    }
                }
            }
            else
                UI2DRenderer.WriteText(Vector2.Zero, central.getPatientName(patients[current].MarkerID), Color.Red, sampleFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.Bottom);
            if (patients[current].MarkerFound == true)
            {
                /*not active until you focus on pt marker for duration of time=maxDropOut*/
                if (dropOut < maxDropOut)
                {
                    dropOut++;
                    if (dropOut == maxDropOut)
                        found = found * -1;
                }
                /*activates if the user has focused on marker for duration of time = maxDropOut*/
                else
                {
                    //if activates patient, add images to render
                    if (found == 1)
                    {
                        UI2DRenderer.WriteText(Vector2.Zero, "Patient Found", Color.White, sampleFont);
                        PatientMarkerNode.AddChild(central.getPatientNameNode(PatientMarkerNode.MarkerID));
                        transformationHsh = central.getInformation(PatientMarkerNode.MarkerID);
                        AddInformation(transformationHsh);
                        
                    }
                    //else if patient deactivated, remove the childrent to stop rendering
                    else
                    {
                        //EquipmentMarkerNode.RemoveChildren();
                        PatientMarkerNode.RemoveChildren();
                        RemoveInformation();
                    }
                    //start counter again
                    dropOut = 0;
                }
            }
            else
            {
                if (found == -1)
                    UI2DRenderer.WriteText(Vector2.Zero, "Patient marker not found", Color.Red, sampleFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.Bottom);
                dropOut = 0;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            UI2DRenderer.WriteText(Vector2.Zero, "Patients: " + patients.Count, Color.Red, sampleFont);
            //UI2DRenderer.WriteText(Vector2.Zero, marker, Color.White, sampleFont);
        }
    }
}
