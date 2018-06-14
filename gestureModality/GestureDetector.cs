namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    using System;
    using Microsoft.Kinect;
    using Microsoft.Kinect.VisualGestureBuilder;
    using mmisharp;

    public class GestureDetector : IDisposable
    {
        private LifeCycleEvents lce;
        private MmiCommunication mmic;

        // Path to databse
        private readonly string gestureDatabase = @"Database\spotify_gestures.gbd";

        // Name of gestures
        private readonly string stop = "stop";
        private readonly string skip = "skip";
        private readonly string back = "back";
        private readonly string vdown = "vdown";
        private readonly string vup = "vup";

        // Gesture frame source which should be tied to a body tracking ID
        private VisualGestureBuilderFrameSource vgbFrameSource = null;

        // Gesture frame reader which will handle gesture events coming from the sensor
        private VisualGestureBuilderFrameReader vgbFrameReader = null;

        private int count;

        // Initializes a new instance of the GestureDetector class along with the gesture frame source and reader
        public GestureDetector(KinectSensor kinectSensor, GestureResultView gestureResultView)
        {
            //init LifeCycleEvents..
            lce = new LifeCycleEvents("GESTURES", "FUSION", "gestures-1", "acoustic", "command"); // LifeCycleEvents(string source, string target, string id, string medium, string mode)
            mmic = new MmiCommunication("localhost",9876,"User1", "ASR");  //PORT TO FUSION - uncomment this line to work with fusion later
            //mmic = new MmiCommunication("localhost", 8000, "User1", "GESTURES"); // MmiCommunication(string IMhost, int portIM, string UserOD, string thisModalityName)
            mmic.Send(lce.NewContextRequest());
            count = 0;

            if (kinectSensor == null)
            {
                throw new ArgumentNullException("Kinect Sensor is null");
            }

            if (gestureResultView == null)
            {
                throw new ArgumentNullException("Gesture Result View is null");
            }

            GestureResultView = gestureResultView;

            // Create the vgb source. The associated body tracking ID will be set when a valid body frame arrives from the sensor.
            vgbFrameSource = new VisualGestureBuilderFrameSource(kinectSensor, 0);
            vgbFrameSource.TrackingIdLost += Source_TrackingIdLost;

            // Open the reader for the vgb frames
            vgbFrameReader = vgbFrameSource.OpenReader();
            if (vgbFrameReader != null)
            {
                vgbFrameReader.IsPaused = true;
                vgbFrameReader.FrameArrived += Reader_GestureFrameArrived;
            }

            // Load gestures from database
            using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(gestureDatabase))
            {
                foreach (Gesture gesture in database.AvailableGestures)
                {
                    if (gesture.Name.Equals(stop) || gesture.Name.Equals(back) || gesture.Name.Equals(skip)
                        || gesture.Name.Equals(vdown) || gesture.Name.Equals(vup))
                    {
                        vgbFrameSource.AddGesture(gesture);
                    }
                }
            }
        }

        // Get GestureResultView object which stores the detector results for display in the UI
        public GestureResultView GestureResultView { get; private set; }

        /*  Get or set the body tracking ID associated with the current detector
            The tracking ID can change whenever a body comes in/out of scope */
        public ulong TrackingId
        {
            get
            {
                return vgbFrameSource.TrackingId;
            }

            set
            {
                if (vgbFrameSource.TrackingId != value)
                {
                    vgbFrameSource.TrackingId = value;
                }
            }
        }

        /*  Get or set a value indicating whether or not the detector is currently paused
            If the body tracking ID associated with the detector is not valid, then the detector should be paused */
        public bool IsPaused
        {
            get
            {
                return vgbFrameReader.IsPaused;
            }

            set
            {
                if (vgbFrameReader.IsPaused != value)
                {
                    vgbFrameReader.IsPaused = value;
                }
            }
        }

        // Disposes all unmanaged resources for the class
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Dispose the VisualGestureBuilderFrameSource and VisualGestureBuilderFrameReader objects
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (vgbFrameReader != null)
                {
                    vgbFrameReader.FrameArrived -= Reader_GestureFrameArrived;
                    vgbFrameReader.Dispose();
                    vgbFrameReader = null;
                }

                if (vgbFrameSource != null)
                {
                    vgbFrameSource.TrackingIdLost -= Source_TrackingIdLost;
                    vgbFrameSource.Dispose();
                    vgbFrameSource = null;
                }
            }
        }

        // Handle gesture detection results arriving from the sensor for the associated body tracking Id
        private void Reader_GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {
            VisualGestureBuilderFrameReference frameReference = e.FrameReference;
            using (var frame = vgbFrameReader.CalculateAndAcquireLatestFrame())
            {
                bool anyGestureDetected = false;
                bool stopDetected = false;
                bool skipDetected = false;
                bool backDetected = false;
                bool vupDetected = false;
                bool vdownDetected = false;
                float progress = 0;

                if (frame != null)
                {
                    // Get gestures results
                    var discreteResults = frame.DiscreteGestureResults;
                    var continuousResults = frame.ContinuousGestureResults;

                    if (discreteResults != null)
                    {
                        foreach (Gesture gesture in vgbFrameSource.Gestures)
                        {
                            if (gesture.GestureType == GestureType.Discrete)
                            {
                                DiscreteGestureResult result = null;
                                discreteResults.TryGetValue(gesture, out result);

                                if (result != null)
                                {
                                    Console.WriteLine("Discrete Gesture");
                                    anyGestureDetected = false;
                                }
                            }
                        }
                    }

                    if (continuousResults != null)
                    {
                        foreach (Gesture gesture in vgbFrameSource.Gestures)
                        {
                            if (gesture.Name.Equals(stop) || gesture.Name.Equals(back) || gesture.Name.Equals(skip)
                                || gesture.Name.Equals(vdown) || gesture.Name.Equals(vup))
                            {
                                ContinuousGestureResult result = null;
                                continuousResults.TryGetValue(gesture, out result);

                                if (result != null)
                                {
                                    progress = result.Progress;
                                        if (progress >= 1)
                                        {
                                        count++;
                                        if(count != 15)
                                        {
                                            return;
                                        }
                                        count = 0;
                                            if (gesture.Name.Equals(stop))
                                            {
                                                sendMessage("PAUSE", progress);
                                                anyGestureDetected = true;
                                                stopDetected = true;
                                                skipDetected = false;
                                                backDetected = false;
                                                vupDetected = false;
                                                vdownDetected = false;
                                            }
                                            else if (gesture.Name.Equals(skip))
                                            {
                                                sendMessage("BACK", progress);
                                                anyGestureDetected = true;
                                                stopDetected = false;
                                                skipDetected = true;
                                                backDetected = false;
                                                vupDetected = false;
                                                vdownDetected = false;
                                            }
                                            else if (gesture.Name.Equals(back))
                                            {
                                                sendMessage("SKIP", progress);
                                                anyGestureDetected = true;
                                                stopDetected = false;
                                                skipDetected = false;
                                                backDetected = true;
                                                vupDetected = false;
                                                vdownDetected = false;
                                            }
                                            else if (gesture.Name.Equals(vup))
                                            {
                                                sendMessage("VUP", progress);
                                                anyGestureDetected = true;
                                                stopDetected = false;
                                                skipDetected = false;
                                                backDetected = false;
                                                vupDetected = true;
                                                vdownDetected = false;
                                            }
                                            else if (gesture.Name.Equals(vdown))
                                            {
                                                sendMessage("VDOWN", progress);
                                                anyGestureDetected = true;
                                                stopDetected = false;
                                                skipDetected = false;
                                                backDetected = false;
                                                vupDetected = false;
                                                vdownDetected = true;
                                            }
                                        }
                                    }
                                }
                            }
                    }
                    GestureResultView.UpdateGestureResult(true, anyGestureDetected, stopDetected, skipDetected,
                                                            backDetected, vupDetected, vdownDetected, progress);
                }
            }
        }

        // Handle the TrackingIdLost event for the VisualGestureBuilderSource object
        private void Source_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            // Update the GestureResultView object to show the 'Not Tracked' image in the UI
            GestureResultView.UpdateGestureResult(false, false, false, false, false, false, false, 0.0f);
        }

        // Send JSON message indicating the parameters in use
        private void sendMessage(string gesture, double confidence)
        {
            string json = "{ \"recognized\": [";
            json += "\"" + gesture + "\", ";
            // Just using the first two comands. The rest is EMP
            for (int i = 1; i < 8; i++)
            {
                json += "\"" + "EMP" + "\", ";
            }
            json = json.Substring(0, json.Length - 2);
            json += "] }";
            var exNot = lce.ExtensionNotification("", "", 1, json);
            mmic.Send(exNot);
        }
    }
}
