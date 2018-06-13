namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;

    // Visualize  Kinect Body stream for display in the UI
    public sealed class KinectBodyView
    {
        // Radius of drawn hand circles
        private const double HandSize = 30;

        // Thickness of drawn joint lines
        private const double JointThickness = 3;

        // Thickness of clip edge rectangles
        private const double ClipBoundsThickness = 10;

        // Constant for clamping Z values of camera space points from being negative
        private const float InferredZPositionClamp = 0.1f;

        // Brush used for drawing hands that are currently tracked as closed
        private readonly Brush handClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        // Brush used for drawing hands that are currently tracked as opened
        private readonly Brush handOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));

        // Brush used for drawing hands that are currently tracked as in lasso (pointer) position
        private readonly Brush handLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));

        // Brush used for drawing joints that are currently tracked
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        // Brush used for drawing joints that are currently inferred      
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        // Pen used for drawing bones that are currently inferred
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        // Draw group for body rendering output
        private DrawingGroup drawingGroup;

        /// Draw image that we will display
        private DrawingImage imageSource;

        /// Coordinate mapper to map one type of point to another
        private CoordinateMapper coordinateMapper = null;

        // Definition of bones
        private List<Tuple<JointType, JointType>> bones;

        /// Width of display (depth space)
        private int displayWidth;

        /// Height of display (depth space)
        private int displayHeight;

        /// List of colors for each body tracked
        private List<Pen> bodyColors;

        // Initializes new instance of the KinectBodyView class
        public KinectBodyView(KinectSensor kinectSensor)
        {
            if (kinectSensor == null)
            {
                throw new ArgumentNullException("Kinect Sensor is null");
            }

            // Get the coordinate mapper
            coordinateMapper = kinectSensor.CoordinateMapper;

            // Get the depth (display) extents
            FrameDescription frameDescription = kinectSensor.DepthFrameSource.FrameDescription;

            // Get size of joint space
            displayWidth = frameDescription.Width;
            displayHeight = frameDescription.Height;

            // A bone defined as a line between two joints
            bones = new List<Tuple<JointType, JointType>>();

            // Torso
            bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

            // Right Arm
            bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

            // Left Arm
            bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));

            // Right Leg
            bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

            // Left Leg
            bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
           bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));

            // Populate body colors, one for each BodyIndex
            bodyColors = new List<Pen>();

            bodyColors.Add(new Pen(Brushes.Red, 6));
            bodyColors.Add(new Pen(Brushes.Orange, 6));
            bodyColors.Add(new Pen(Brushes.Green, 6));
            bodyColors.Add(new Pen(Brushes.Blue, 6));
            bodyColors.Add(new Pen(Brushes.Indigo, 6));
            bodyColors.Add(new Pen(Brushes.Violet, 6));

            // Create the drawing group used for drawing
            drawingGroup = new DrawingGroup();

            // Create an image source that is used in image control
            imageSource = new DrawingImage(drawingGroup);
        }

        // Get the bitmap to display
        public ImageSource ImageSource
        {
            get
            {
                return imageSource;
            }
        }

        /*  Update the body array with new information from the sensor
            Should be called whenever a new BodyFrameArrivedEvent occurs */
        public void UpdateBodyFrame(Body[] bodies)
        {
            if (bodies != null)
            {
                using (DrawingContext dc = drawingGroup.Open())
                {
                    // Draw a transparent background to set the render size
                    dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, displayWidth, displayHeight));

                    int penIndex = 0;
                    foreach (Body body in bodies)
                    {
                        Pen drawPen = bodyColors[penIndex++];

                        if (body.IsTracked)
                        {
                            DrawClippedEdges(body, dc);

                            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                            // convert the joint points to depth (display) space
                            Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                            foreach (JointType jointType in joints.Keys)
                            {
                                // ometimes the depth(Z) of an inferred joint may show as negative
                                // Clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
                                CameraSpacePoint position = joints[jointType].Position;
                                if (position.Z < 0)
                                {
                                    position.Z = InferredZPositionClamp;
                                }

                                DepthSpacePoint depthSpacePoint = coordinateMapper.MapCameraPointToDepthSpace(position);
                                jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                            }

                            DrawBody(joints, jointPoints, dc, drawPen);

                            DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                            DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);
                        }
                    }

                    // Prevent drawing outside of our render area
                    drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, displayWidth, displayHeight));
                }
            }
        }

        // Draw a body
        private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext, Pen drawingPen)
        {
            // Draw the bones
            foreach (var bone in bones)
            {
                DrawBone(joints, jointPoints, bone.Item1, bone.Item2, drawingContext, drawingPen);
            }

            // Draw the joints
            foreach (JointType jointType in joints.Keys)
            {
                Brush drawBrush = null;

                TrackingState trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    drawBrush = trackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    drawBrush = inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }
            }
        }

        // Draw one bone of a body (joint to joint)
        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Pen drawingPen)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If these joints are not found, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // Assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = inferredBonePen;
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
            {
                drawPen = drawingPen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }

        // Draw a hand symbol if the hand is tracked: red circle = closed, green circle = opened; blue circle = lasso
        private void DrawHand(HandState handState, Point handPosition, DrawingContext drawingContext)
        {
            switch (handState)
            {
                case HandState.Closed:
                    drawingContext.DrawEllipse(handClosedBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Open:
                    drawingContext.DrawEllipse(handOpenBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Lasso:
                    drawingContext.DrawEllipse(handLassoBrush, null, handPosition, HandSize, HandSize);
                    break;
            }
        }

        // Draw indicators to show which edges are clipping body data
        private void DrawClippedEdges(Body body, DrawingContext drawingContext)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, displayHeight - ClipBoundsThickness, displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, displayHeight));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(displayWidth - ClipBoundsThickness, 0, ClipBoundsThickness, displayHeight));
            }
        }
    }
}
