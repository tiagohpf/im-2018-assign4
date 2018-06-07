namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Kinect;

    // Interaction logic for the MainWindow
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Active Kinect sensor
        private KinectSensor kinectSensor = null;
        
        // Array for the bodies (Kinect will track up to 6 people simultaneously)
        private Body[] bodies = null;

        // Reader for body frames
        private BodyFrameReader bodyFrameReader = null;

        // Current status text to display
        private string statusText = null;

        // KinectBodyView object which handles drawing the Kinect bodies to a View box in the UI
        private KinectBodyView kinectBodyView = null;
        
        // List of gesture detectors, there will be one detector created for each potential body (max of 6)
        private List<GestureDetector> gestureDetectorList = null;

        // Initialize new instance of the MainWindow class
        public MainWindow()
        {
            // Only one sensor is currently supported
            kinectSensor = KinectSensor.GetDefault();
            
            // Set IsAvailableChanged event notifier
            kinectSensor.IsAvailableChanged += Sensor_IsAvailableChanged;

            // Open the sensor
            kinectSensor.Open();

            // Set the status text
            StatusText = kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.NoSensorStatusText;

            // Open the reader for the body frames
            bodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();

            // Set the BodyFramedArrived event notifier
            bodyFrameReader.FrameArrived += Reader_BodyFrameArrived;

            // Initialize the BodyViewer object for displaying tracked bodies in the UI
            kinectBodyView = new KinectBodyView(kinectSensor);

            // Initialize the gesture detection objects for our gestures
            gestureDetectorList = new List<GestureDetector>();

            // Initialize the MainWindow
            InitializeComponent();

            // Set data context objects for display in UI
            DataContext = this;
            kinectBodyViewbox.DataContext = kinectBodyView;

            // Create a gesture detector for each body (6 bodies => 6 detectors) and create content controls to display results in the UI
            int col0Row = 0;
            int col1Row = 0;
            int maxBodies = kinectSensor.BodyFrameSource.BodyCount;
            for (int i = 0; i < maxBodies; ++i)
            {
                GestureResultView result = new GestureResultView(i, false, false, 0.0f);
                GestureDetector detector = new GestureDetector(kinectSensor, result);
                gestureDetectorList.Add(detector);                
                
                // Split gesture results across the first two columns of the content grid
                ContentControl contentControl = new ContentControl();
                contentControl.Content = gestureDetectorList[i].GestureResultView;
                
                if (i % 2 == 0)
                {
                    // Gesture results for bodies: 0, 2, 4
                    Grid.SetColumn(contentControl, 0);
                    Grid.SetRow(contentControl, col0Row);
                    ++col0Row;
                }
                else
                {
                    // Gesture results for bodies: 1, 3, 5
                    Grid.SetColumn(contentControl, 1);
                    Grid.SetRow(contentControl, col1Row);
                    ++col1Row;
                }

                contentGrid.Children.Add(contentControl);
            }
        }

        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        public event PropertyChangedEventHandler PropertyChanged;

        /// Get or set the current status text to display
        public string StatusText
        {
            get
            {
                return statusText;
            }

            set
            {
                if (statusText != value)
                {
                    statusText = value;

                    // Notify any bound elements that the text has changed
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }

        // Execute shutdown tasks
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                bodyFrameReader.FrameArrived -= Reader_BodyFrameArrived;
                bodyFrameReader.Dispose();
                bodyFrameReader = null;
            }

            if (gestureDetectorList != null)
            {
                // The GestureDetector contains disposable members (VisualGestureBuilderFrameSource and VisualGestureBuilderFrameReader)
                foreach (GestureDetector detector in gestureDetectorList)
                {
                    detector.Dispose();
                }

                gestureDetectorList.Clear();
                gestureDetectorList = null;
            }

            if (kinectSensor != null)
            {
                kinectSensor.IsAvailableChanged -= Sensor_IsAvailableChanged;
                kinectSensor.Close();
                kinectSensor = null;
            }
        }

        // Handle event when the sensor becomes unavailable (e.g. paused, closed, unplugged).
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // On failure, set the status text
            StatusText = kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.SensorNotAvailableStatusText;
        }

        // Handle body frame data arriving from the sensor and updates the associated gesture detector object for each body
        private void Reader_BodyFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (bodies == null)
                    {
                        // Create array of 6 bodies, which is the max number of bodies that Kinect can track simultaneously
                        bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // Those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                // Visualize the new body data
                kinectBodyView.UpdateBodyFrame(bodies);

                // It may have lost/acquired bodies, so update the corresponding gesture detectors
                if (bodies != null)
                {
                    // Loop through all bodies to see if any of the gesture detectors need to be updated
                    int maxBodies = kinectSensor.BodyFrameSource.BodyCount;
                    for (int i = 0; i < maxBodies; ++i)
                    {
                        Body body = bodies[i];
                        ulong trackingId = body.TrackingId;

                        // If the current body TrackingId changed, update the corresponding gesture detector with the new value
                        if (trackingId != gestureDetectorList[i].TrackingId)
                        {
                            gestureDetectorList[i].TrackingId = trackingId;

                            // If the current body is tracked, unpause its detector to get VisualGestureBuilderFrameArrived events
                            // If the current body is not tracked, pause its detector so it don't waste resources trying to get invalid gesture results
                            gestureDetectorList[i].IsPaused = trackingId == 0;
                        }
                    }
                }
            }
        }
    }
}
