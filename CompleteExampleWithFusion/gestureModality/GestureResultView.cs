namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /*  Store discrete gesture results for the GestureDetector.
        Properties are stored/updated for display in the UI. */
    public sealed class GestureResultView : INotifyPropertyChanged
    {
        // Images for detector
        private readonly ImageSource stopImage = new BitmapImage(new Uri(@"Images\Pause.png", UriKind.Relative));
        private readonly ImageSource skipImage = new BitmapImage(new Uri(@"Images\Skip.png", UriKind.Relative));
        private readonly ImageSource backImage = new BitmapImage(new Uri(@"Images\Back.png", UriKind.Relative));
        private readonly ImageSource vupImage = new BitmapImage(new Uri(@"Images\Volume Up.png", UriKind.Relative));
        private readonly ImageSource vdownImage = new BitmapImage(new Uri(@"Images\Volume Down.png", UriKind.Relative));

        // Image to show when the 'detected' property is false for a tracked body
        private readonly ImageSource notDetectedImage = new BitmapImage(new Uri(@"Images\None.png", UriKind.Relative));

        // Image to show when the body associated with the GestureResultView object is not being tracked
        private readonly ImageSource notTrackedImage = new BitmapImage(new Uri(@"Images\NotTracked.png", UriKind.Relative));

        // Array of brush colors to use for a tracked body; array position corresponds to the body colors used in the KinectBodyView class
        private readonly Brush[] trackedColors = new Brush[] { Brushes.Red, Brushes.Orange, Brushes.Green, Brushes.Blue, Brushes.Indigo, Brushes.Violet };

        // Brush color to use as background in the UI
        private Brush bodyColor = Brushes.Gray;

        // The body index (0-5) associated with the current gesture detector
        private int bodyIndex = 0;

        // Current confidence value reported by the discrete gesture
        private float confidence = 0.0f;

        // True, if the discrete gesture is currently being detected
        private bool detected = false;

        // Image to display in UI which corresponds to tracking/detection state
        private ImageSource imageSource = null;
        
        // True, if the body is currently being tracked
        private bool isTracked = false;

        // Initialize a new instance of the GestureResultView class and sets initial property values
        public GestureResultView(int bodyIndex, bool isTracked, bool detected, float confidence)
        {
            BodyIndex = bodyIndex;
            IsTracked = isTracked;
            Detected = detected;
            Confidence = confidence;
            ImageSource = notTrackedImage;
        }

        // INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        public event PropertyChangedEventHandler PropertyChanged;

        // Get the body index associated with the current gesture detector result 
        public int BodyIndex
        {
            get
            {
                return bodyIndex;
            }

            private set
            {
                if (bodyIndex != value)
                {
                    bodyIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// Get the body color corresponding to the body index for the result
        public Brush BodyColor
        {
            get
            {
                return bodyColor;
            }

            private set
            {
                if (bodyColor != value)
                {
                    bodyColor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// Get a value indicating whether or not the body associated with the gesture detector is currently being tracked 
        public bool IsTracked 
        {
            get
            {
                return isTracked;
            }

            private set
            {
                if (IsTracked != value)
                {
                    isTracked = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// Get a value indicating whether or not the discrete gesture has been detected
        public bool Detected 
        {
            get
            {
                return detected;
            }

            private set
            {
                if (detected != value)
                {
                    detected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// Get a float value which indicates the detector's confidence that the gesture is occurring for the associated body 
        public float Confidence
        {
            get
            {
                return confidence;
            }

            private set
            {
                if (confidence != value)
                {
                    confidence = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // Gets an image for display in the UI which represents the current gesture result for the associated body 
        public ImageSource ImageSource
        {
            get
            {
                return imageSource;
            }

            private set
            {
                if (ImageSource != value)
                {
                    imageSource = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // Update the values associated with the discrete gesture detection result
        public void UpdateGestureResult(bool isBodyTrackingIdValid, bool anyGestureDetected, bool stopDetected, bool skipDetected, 
                                        bool backDetected, bool vupDetected, bool vdownDetected, float detectionConfidence)
        {
            IsTracked = isBodyTrackingIdValid;
            Confidence = 0.0f;

            if (!IsTracked)
            {
                ImageSource = notTrackedImage;
                Detected = false;
                BodyColor = Brushes.Gray;
            }
            else
            {
                Detected = anyGestureDetected;
                BodyColor = trackedColors[BodyIndex];

                if (Detected)
                {
                    Confidence = detectionConfidence;
                    if (stopDetected)
                    {
                        ImageSource = stopImage;
                    }
                    else if (skipDetected)
                    {
                        ImageSource = backImage;
                    }
                    else if (backDetected)
                    {
                        ImageSource = skipImage;
                    }
                    else if (vupDetected)
                    {
                        ImageSource = vupImage;
                    }
                    else if (vdownDetected)
                    {
                        ImageSource = vdownImage;
                    }
                }
                else
                {
                    ImageSource = notDetectedImage;
                }
            }
        }

        // Notify UI that a property has changed
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
