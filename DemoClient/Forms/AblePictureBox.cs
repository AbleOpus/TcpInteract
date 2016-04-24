using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DemoClient.Forms
{
    /// <summary>
    /// Specifies how to draw a rectangular element.
    /// </summary>
    public enum DisplayMode
    {
        /// <summary>
        /// Will be shown as is.
        /// </summary>
        Normal,
        /// <summary>
        /// The image will be expanded to just fill all of the unused space.
        /// </summary>
        Fill,
        /// <summary>
        /// The image will be expanded centered.
        /// </summary>
        Center,
        /// <summary>
        /// The image will be rescaled to make the bounds of the control.
        /// </summary>
        Stretch,
        /// <summary>
        /// The image will be initially normal. But will be input zoomable.
        /// </summary>
        Zoomable
    }

    /// <summary>
    /// Represents an advanced picture box control, with zooming and drag-drop functionality.
    /// </summary>
    [DesignTimeVisible(true), DefaultProperty("Image"), DefaultEvent("ImageLoaded")]
    public class AblePictureBox : Control
    {
        private RectangleF imageRect;
        private Point lastPos; // In client coords
        private Point mouseDownPos; // In screen coords
        private Size lastSize;
        private float diff;

        #region Properties
        private Image image;
        /// <summary>
        /// Gets or sets the image to display.
        /// </summary>
        [Category("Appearance"), Description("Determines the image to display.")]
        [DefaultValue(null)]
        public Image Image
        {
            get { return image; }
            set
            {
                if (image != value)
                {
                    Image oldImage = (Image) image?.Clone();
                    image = value;
                    OnImageChanging();
                    UpdateImageRectangle();

                    if (image != null && mode == DisplayMode.Zoomable)
                        SetImageRectToFillUncentered();

                    OnImageChanged(oldImage);
                    Invalidate();
                }
            }
        }

        private DisplayMode mode = DisplayMode.Center;
        /// <summary>
        /// Gets or sets how the image will be displayed.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(DisplayMode.Center)]
        [Description("Determines how the image will be displayed.")]
        public DisplayMode SizeMode
        {
            get { return mode; }
            set
            {
                if (value != mode)
                {
                    mode = value;

                    // if changing from zoomable then set cursor to default
                    if (mode == DisplayMode.Zoomable)
                    {
                        Cursor = Cursors.Default;
                        if (image != null) SetImageRectToFillUncentered();
                    }

                    Invalidate();
                }
            }
        }

        [Category("Behavior"), DefaultValue(10)]
        [Description("Determines how fast to zoom in zoom mode. Its recommended to keep this value between 0 and 15.")]
        public int ZoomSpeedMultiplier { get; set; } = 10;

        protected override Size DefaultSize => new Size(300, 300);
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AblePictureBox"/> class.
        /// </summary>
        public AblePictureBox()
        {
            ResizeRedraw = true;
            DoubleBuffered = true;
        }

        #region Overrides
        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);
            var fileNames = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (fileNames != null && fileNames.Length > 0)
            {
                // Load the first in the argument
                LoadImage(fileNames[0]);
                FilesDropped(this, fileNames);
            }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            var fileNames = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (fileNames != null && fileNames.Length > 0)
            {
                if (FileTypeSupported(Path.GetExtension(fileNames[0])))
                    e.Effect = DragDropEffects.All;
            }
        }

        protected override void OnContextMenuStripChanged(EventArgs e)
        {
            base.OnContextMenuStripChanged(e);
            if (ContextMenuStrip == null) return;

            ContextMenuStrip.Opening += (s, e2) =>
            {
                // Cancel opening if the user has moved the mouse 5 on the x or y
                const int THRESHOLD = 5;
                bool movedOnX = Math.Abs(mouseDownPos.X - Cursor.Position.X) > THRESHOLD;
                bool movedOnY = Math.Abs(mouseDownPos.Y - Cursor.Position.Y) > THRESHOLD;
                if (movedOnX || movedOnY) e2.Cancel = true;
            };
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Cursor = Cursors.Default;

            if (mode == DisplayMode.Zoomable)
            {
                AlignImageRectangle();
                Invalidate();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            UpdateImageRectangle();
            lastSize = Size;

            if (mode == DisplayMode.Zoomable)
                AlignImageRectangle();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (image == null) return;

            switch (mode)
            {
                case DisplayMode.Normal:
                    e.Graphics.DrawImage(image, new Rectangle(Point.Empty, image.Size));
                    break;

                case DisplayMode.Stretch:
                    e.Graphics.DrawImage(image, ClientRectangle);
                    break;

                case DisplayMode.Zoomable:
                    if (image != null && imageRect.Width > 0 && imageRect.Height > 0)
                        e.Graphics.DrawImage(image, imageRect);
                    break;

                case DisplayMode.Center: DrawImageCentered(e.Graphics); break;
                case DisplayMode.Fill: DrawImageFilled(e.Graphics); break;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left)
            {
                if (image != null && mode == DisplayMode.Zoomable &&
                    Cursor == Cursors.Default)
                {
                    Cursor = Cursors.SizeAll;
                }

                float x = imageRect.Left + e.X - lastPos.X;
                float y = imageRect.Top + e.Y - lastPos.Y;
                imageRect.Location = new PointF(x, y);
                lastPos = e.Location;
                Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                diff = (e.Y - lastPos.Y) * ZoomSpeedMultiplier;
                float height = imageRect.Height + diff;
                float ratio = height / imageRect.Height;
                // We need to alter the width according to the change in height
                float width = imageRect.Width * ratio;
                // Set x and y so the image sizes around its center
                float x = imageRect.X + (imageRect.Width - width) / 2f;
                float y = imageRect.Y + (imageRect.Height - height) / 2f;
                // makes for cleaner zooming (anchor top or left under certain circumstances)
                if (x > 0 || imageRect.Width < Width) x = 0;
                if (y > 0 || imageRect.Height < Height) y = 0;

                // If not shrinking and image is above 3x3 pixels
                if (!(diff < 0 && (width < 30 || height < 30)))
                {
                    imageRect = new RectangleF(x, y, width, height);
                }

                lastPos = e.Location;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            lastPos = e.Location;

            // If there is an image and in zoom mode and button is left
            if (image != null && mode == DisplayMode.Zoomable)
            {
                if (e.Button == MouseButtons.Left)
                    Cursor = Cursors.SizeAll;
                else if (e.Button == MouseButtons.Right)
                    Cursor = Cursors.NoMoveVert;
            }

            if (e.Button == MouseButtons.Right)
                mouseDownPos = Cursor.Position;
        }
        #endregion

        /// <summary>
        /// Occurs when the value of the Image property has been changed but before any sizing logic.
        /// </summary>
        [Description("Occurs when the value of the Image property has been changed but before any sizing logic.")]
        public event EventHandler ImageChanging;
        /// <summary>
        /// Raises the <see cref="ImageChanging"/> event.
        /// </summary>
        protected virtual void OnImageChanging()
        {
            ImageChanging?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the value of the Image property has changed.
        /// </summary>
        [Description("Occurs when the value of the Image property has changed.")]
        public event EventHandler<Image> ImageChanged;
        /// <summary>
        /// Raises the <see cref="ImageChanged"/> event.
        /// </summary>
        protected virtual void OnImageChanged(Image img)
        {
            ImageChanged?.Invoke(this, img);
        }

        /// <summary>
        /// Occurs when files have been dropped onto the control.
        /// </summary>
        [Description("Occurs when files have been dropped onto the control.")]
        public event EventHandler<string[]> FilesDropped = delegate { };
        /// <summary>
        /// Raises the <see cref="FilesDropped"/> event.
        /// </summary>
        protected virtual void OnFilesDropped(string[] fileNames)
        {
            FilesDropped?.Invoke(this, fileNames);
        }

        /// <summary>
        /// Occurs when an Image is loaded from file.
        /// </summary>
        [Description("Occurs when an Image is loaded from file.")]
        public event EventHandler<ImageLoadedEventArgs> ImageLoaded;
        /// <summary>
        /// Raises the <see cref="ImageLoaded"/> event.
        /// </summary>
        protected virtual void OnImageLoaded(string fileName, Exception error)
        {
            var args = new ImageLoadedEventArgs(fileName, error);
            ImageLoaded?.Invoke(this, args);
        }

        /// <summary>
        /// Load an image from file.
        /// </summary>
        public void LoadImage(string fileName)
        {
            Exception error = null;
            const StringComparison OPTIONS = StringComparison.OrdinalIgnoreCase;

            try
            {
                // If executable then extract its icon and convert it to a bitmap
                if (Path.GetExtension(fileName).Equals(".exe", OPTIONS))
                {
                    Icon icon = Icon.ExtractAssociatedIcon(fileName);
                    if (icon != null) Image = icon.ToBitmap();
                }
                else if (Path.GetExtension(fileName).Equals(".ico", OPTIONS))
                {
                    Icon icon = new Icon(fileName);
                    Image = icon.ToBitmap();
                }
                else // Normal file format
                {
                    Image = Image.FromFile(fileName);
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            OnImageLoaded(fileName, error);
        }

        /// <summary>
        /// Gets whether or not the accepted extension is of a supported image file.
        /// </summary>
        /// <returns>Whether the given filename seems to imply a supported filetype.</returns>
        public static bool FileTypeSupported(string ext)
        {
            string[] SUPPORTED = { ".jpg", ".jpeg", ".bmp", ".png", ".tiff", ".gif", ".exe", ".ico" };
            return SUPPORTED.Any(pattern => ext.Equals(pattern, StringComparison.OrdinalIgnoreCase));
        }

        private void UpdateImageRectangle()
        {
            if (mode == DisplayMode.Zoomable)
            {
                if (imageRect.Width > imageRect.Height)
                {
                    if (lastSize.Width < ClientSize.Width && imageRect.Width < Width)
                    {
                        ScaleImageByWidth(Width);
                    }
                }
                else if (lastSize.Height < ClientSize.Height && imageRect.Height < Height)
                {
                    ScaleImageByHeight(Height);
                }
            }
        }

        /// <summary>
        /// Set image width and retain aspect ratio.
        /// </summary>
        private void ScaleImageByWidth(int width)
        {
            double ratio = (double)width / imageRect.Width;
            imageRect.Width = width;
            imageRect.Height = (float)(imageRect.Height * ratio);
        }

        /// <summary>
        /// Set image height and retain aspect ratio.
        /// </summary>
        private void ScaleImageByHeight(int height)
        {
            double ratio = (double)height / imageRect.Height;
            imageRect.Height = height;
            imageRect.Width = (float)(imageRect.Width * ratio);
        }

        private void AlignImageRectangle()
        {
            bool smallWidth = false;
            bool smallHeight = false;

            // Test to see if the image is smaller than the control and the width is the biggest dimension.
            if (imageRect.Width > imageRect.Height && imageRect.Width < Width)
            {
                smallWidth = true;
                imageRect.Location = new PointF(0, imageRect.Y);
                ScaleImageByWidth(Width);
            }
            else if (imageRect.X > 0) // We do not want to drag to far right
            {
                imageRect.Location = new PointF(0, imageRect.Location.Y);
            }
            else if (imageRect.X + imageRect.Width < ClientSize.Width) // We do not want to drag to far left
            {
                if (imageRect.Width >= Width)
                    imageRect.Location = new PointF(Width - imageRect.Width, imageRect.Location.Y);
                else
                    imageRect.Location = new PointF(0, imageRect.Location.Y);
            }

            // Test to see if the image is smaller than the control and the height is the biggest dimension.
            if (imageRect.Width < imageRect.Height && imageRect.Height < ClientSize.Height)
            {
                smallHeight = true;
                imageRect.Location = new PointF(imageRect.X, 0);
                ScaleImageByHeight(ClientSize.Height);
            }
            else if (imageRect.Y > 0) // We do not want to drag to far down
                imageRect.Location = new PointF(imageRect.Location.X, 0);
            else if (imageRect.Y + imageRect.Height < ClientSize.Height) // We do not want to drag to far up
                imageRect.Location = new PointF(imageRect.X, Height - imageRect.Height);

            if (smallHeight && smallWidth) return;

            // Keep us from dragging the image to far up when the image is smaller than the control.
            if (imageRect.Height < Height)
                imageRect.Location = new PointF(imageRect.X, 0);
        }

        private void DrawImageFilled(Graphics graphics)
        {
            float width = Width; // The new image width (for drawing only)
            float height = Height; // The new image height
            float x = 0;
            float y = 0;
            // Get image to control dimension ratio
            float ratio1 = image.Width / width;
            float ratio2 = image.Height / height;

            // Whatever ratio is greatest is the ratio to be applied.
            if (ratio1 < ratio2)
            {
                // Apply ratio to the height of the image (we dont need to touch the width).
                height = image.Height / ratio1;
                // Center the image on the y, it will be left 0 on the x.
                y = Height / 2f - height / 2f;
            }
            else if (ratio2 < ratio1) // do the exact same thing, just for the other dimension
            {
                width = image.Width / ratio2;
                x = Width / 2f - width / 2f;
            }

            graphics.DrawImage(image, x, y, width, height);
        }

        private void DrawImageCentered(Graphics graphics)
        {
            float width = ClientSize.Width; // The new image width (for drawing only)
            float height = ClientSize.Height; // The new image height
            float x = 0;
            float y = 0;
            // Get image to control dimension ratio.
            float ratio1 = image.Width / (float)Width;
            float ratio2 = image.Height / (float)Height;

            // Whatever ratio is greatest is the ratio to be applied.
            if (ratio1 > ratio2)
            {
                // Apply ratio to the height of the image (we don't need to touch the width).
                height = image.Height / ratio1;
                // Center the image on the y, it will be left 0 on the x.
                y = ClientSize.Height / 2f - height / 2f;
            }
            else
            {
                width = image.Width / ratio2;
                x = ClientSize.Width / 2f - width / 2f;
            }

            graphics.DrawImage(image, x, y, width, height);
        }

        /// <summary>
        /// The image rectangle is resized to be completely visible with no centering.
        /// </summary>
        private void SetImageRectToFillUncentered()
        {
            float width = Width; // The new image width (for drawing only)
            float height = Height; // The new image height
            // Get image to control dimension ratio
            float ratio1 = image.Width / width;
            float ratio2 = image.Height / height;

            // Whatever ratio is greatest is the ratio to be applied.
            if (ratio1 > ratio2)
            {
                // Apply ratio to the height of the image (we dont need to touch the width).
                height = image.Height / ratio1;
                // Center the image on the y, it will be left 0 on the x.
            }
            else if (ratio2 > ratio1) // do the exact same thing, just for the other dimension
            {
                width = image.Width / ratio2;
            }

            imageRect = new RectangleF(0, 0, width, height);
        }
    }

    /// <summary>
    /// Event arguments use for the <see cref="AblePictureBox.ImageLoaded"/> event.
    /// </summary>
    public class ImageLoadedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the filename for the image loaded.
        /// </summary>
        public string FileName { get; private set; }
        /// <summary>
        /// Gets an error if something goes wrong.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageLoadedEventArgs"/> class
        /// with the specified arguments.
        /// </summary>
        /// <param name="fileName">The filename for the image loaded.</param>
        /// <param name="error">An error if something goes wrong.</param>
        public ImageLoadedEventArgs(string fileName, Exception error)
        {
            FileName = fileName;
            Error = error;
        }
    }
}
