using NReco.VideoConverter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoWaterMarker
{
    public partial class Form1 : Form
    {
        Stream PreviewImg = new MemoryStream();
        bool FontsSet = false;
        string Title = "Video Watermarker";
        string ConvertedWithWatermark = "ConvertedWithWatermark";

        public Form1()
        {
            InitializeComponent();
        }


        void ConvertFileWithTextOverlayOnly(string file)
        {
            SetTextandGraphics();
            checkSelectedIndex(FormatChooser.SelectedIndex, file);
        }

        public void SetTextandGraphics()
        {
            try
            {
                Font textBrush = new Font(textfont.SelectedItem.ToString(), (int)fontSize.Value);
                //first, create a dummy bitmap just to get a graphics object
                Image img = new Bitmap(1, 1);
                Graphics drawing = Graphics.FromImage(img);

                //measure the string to see how big the image needs to be
                SizeF textSize = drawing.MeasureString(OverlayText.Text.ToString(), textBrush);

                //free up the dummy image and old graphics object
                img.Dispose();
                drawing.Dispose();

                //create a new image of the right size
                img = new Bitmap((int)textSize.Width, (int)textSize.Height);

                drawing = Graphics.FromImage(img);

                //paint the background
                drawing.Clear(Color.Transparent);

                string colorName = textColour.SelectedItem.ToString();
                int Transparency = TransparencySlider.Value;
                Color newColor = Color.FromArgb(Transparency, Color.FromName(colorName));
                SolidBrush brush = new SolidBrush(newColor);

                drawing.DrawString(OverlayText.Text.ToString(), textBrush, brush, 0, 0);
                drawing.Save();

                img.Save(@"text.png", System.Drawing.Imaging.ImageFormat.Png);

                img.Dispose();
                textBrush.Dispose();
                drawing.Dispose();
                brush.Dispose();

            }
            catch (Exception e)
            {
                MessageBox.Show("Exception error has occured:\n" + e.Message.ToString(), Title);
            }
        }
        void ConvertFileWithTextandWatermark(string file)
        {
            SetTextandGraphics();
            try
            {
                using (var srcImage = Image.FromFile(WatermarkFileChooser.FileName))
                {
                    var newWidth = (int)(WatermarkXsize.Value);
                    var newHeight = (int)(WatermarkYsize.Value);
                    using (var newImage = new Bitmap(newWidth, newHeight))
                    using (var graphics = Graphics.FromImage(newImage))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.DrawImage(srcImage, new Rectangle(0, 0, newWidth, newHeight));
                        newImage.Save(@"Resized.png", System.Drawing.Imaging.ImageFormat.Png);
                        newImage.Dispose();
                    }
                    srcImage.Dispose();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception error has occured:\n" + e.Message.ToString(), Title);
            }

            checkSelectedIndex(FormatChooser.SelectedIndex, file);
        }

        public void checkSelectedIndex(int selectedIndex, string file)
        {
            try
            {
                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();

                NReco.VideoConverter.FFMpegInput[] ffMpegInputs = new NReco.VideoConverter.FFMpegInput[] { new FFMpegInput(file), new FFMpegInput(@"text.png") };

                ConvertSettings csettings = new ConvertSettings();

                if (ResizeOptions.Enabled == true)
                {
                    csettings.SetVideoFrameSize((int)Xaxis.Value, (int)Yaxis.Value);
                }

                csettings.AudioCodec = "copy";
                string arguement = "-filter_complex \"overlay=" + textxaxis.Value.ToString() + ":" + textyaxis.Value.ToString() + "\"";

                csettings.CustomOutputArgs = arguement;


                switch (selectedIndex)
                {
                    case 0:
                        ffMpeg.ConvertMedia(ffMpegInputs, file + "ConvertedWithWatermak.avi", Format.avi, csettings);
                        break;
                    case 1:
                        ffMpeg.ConvertMedia(ffMpegInputs, file + "ConvertedWithWatermark.mp4", Format.mp4, csettings);
                        break;
                    case 2:
                        ffMpeg.ConvertMedia(ffMpegInputs, file + "ConvertedWithWatermark.mv4", Format.m4v, csettings);
                        break;
                    case 3:
                        ffMpeg.ConvertMedia(ffMpegInputs, file + "ConvertedWithWatermark.gif", Format.gif, csettings);
                        break;
                    case 4:
                        ffMpeg.ConvertMedia(ffMpegInputs, file + "ConvertedWithWatermark.mov", Format.mov, csettings);
                        break;
                    case 5:
                        ffMpeg.ConvertMedia(ffMpegInputs, file + "ConvertedWithWatermark.wmv", Format.wmv, csettings);
                        break;
                    case 6:
                        ffMpeg.ConvertMedia(ffMpegInputs, file + "ConvertedWithWatermark.swf", Format.swf, csettings);
                        break;
                    case 7:
                        ffMpeg.ConvertMedia(ffMpegInputs, file + "ConvertedWithWatermark.ogg", Format.ogg, csettings);
                        break;
                    case 8:
                        ffMpeg.ConvertMedia(ffMpegInputs, file + "ConvertedWithWatermark.mpeg", Format.mpeg, csettings);
                        break;
                    default:
                        ffMpeg.ConvertMedia(ffMpegInputs, file + "ConvertedWithWatermark.avi", Format.avi, csettings);
                        break;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Exception error has occured:\n" + e.Message.ToString(), Title);
            }

        }

        void ConvertFileWithWatermarkOnly(string file)
        {
            try
            {
                using (var srcImage = Image.FromFile(WatermarkFileChooser.FileName))
                {
                    var newWidth = (int)(WatermarkXsize.Value);
                    var newHeight = (int)(WatermarkYsize.Value);
                    using (var newImage = new Bitmap(newWidth, newHeight))
                    using (var graphics = Graphics.FromImage(newImage))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.DrawImage(srcImage, new Rectangle(0, 0, newWidth, newHeight));
                        newImage.Save(@"Resized.png", System.Drawing.Imaging.ImageFormat.Png);
                        newImage.Dispose();
                    }
                    srcImage.Dispose();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception error has occured:\n" + e.Message.ToString(), Title);
            }

            checkSelectedIndex(FormatChooser.SelectedIndex, file);
        }



        void ConvertFile(string file)
        {
            try
            {
                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                var ffMpegInput = new NReco.VideoConverter.FFMpegInput(file);

                ConvertSettings csettings = new ConvertSettings();
                if (EnableResize.Checked == true)
                {
                    csettings.SetVideoFrameSize((int)Xaxis.Value, (int)Yaxis.Value);
                }

                checkSelectedIndex(FormatChooser.SelectedIndex, file);

            }
            catch (Exception e)
            {
                MessageBox.Show("Exception error has occured:\n" + e.Message.ToString(), Title);
            }
        }

        private void ExportBtn_Click(object sender, EventArgs e)
        {
            if (VideoFileChooser.FileName != "")
            {
                if (EnableWatermarkChkBx.Checked == true && EnableTextOverlayChk.Checked == false)
                {
                    ConvertFileWithWatermarkOnly(VideoFileChooser.FileName.ToString());
                }
                else if (EnableTextOverlayChk.Checked == true && EnableWatermarkChkBx.Checked == false)
                {
                    ConvertFileWithTextOverlayOnly(VideoFileChooser.FileName.ToString());
                }
                else if (EnableTextOverlayChk.Checked == true && EnableWatermarkChkBx.Checked == true)
                {
                    ConvertFileWithTextandWatermark(VideoFileChooser.FileName.ToString());
                }
                else
                {
                    ConvertFile(VideoFileChooser.FileName.ToString());
                }
            }
            else
            {
                MessageBox.Show("You need to choose a video file first!", "Video Watermarker");
            }
        }

        private void ChooseVidBtn_Click(object sender, EventArgs e)
        {
            PreviewImg.SetLength(0);
            PreviewImg.Position = 0;
            VideoFileChooser.Filter = "Video File|*.avi;*.mpeg;*.flv;*.mov;*.ogg;*.mp4;*.wmv;*.mkv;*.h264";
            VideoFileChooser.FileName = "";
            DialogResult result = VideoFileChooser.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(VideoFileChooser.FileName))
            {
                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                var ffMpegInput = new NReco.VideoConverter.FFMpegInput(VideoFileChooser.FileName.ToString());
                ffMpeg.GetVideoThumbnail(VideoFileChooser.FileName.ToString(), PreviewImg);
                ThumbNailPicture.Image = Image.FromStream(PreviewImg);
                EnableWatermarkChkBx.Enabled = true;
            }
        }

        void UpdatePreview()
        {
            Stream tempImg = new MemoryStream();
            using (var srcImage = Image.FromStream(PreviewImg))
            {
                using (var newImage = new Bitmap(srcImage.Width, srcImage.Height))
                using (var graphics = Graphics.FromImage(newImage))
                {
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.DrawImage(srcImage, new Rectangle(0, 0, srcImage.Width, srcImage.Height));

                    newImage.Save(tempImg, System.Drawing.Imaging.ImageFormat.Bmp);
                    newImage.Dispose();
                    srcImage.Dispose();
                }
            }
            if (WatermarkFileChooser.FileName != "")
            {
                using (var srcImage = Image.FromStream(tempImg))
                {
                    using (var newImage = new Bitmap(srcImage.Width, srcImage.Height))
                    using (var graphics = Graphics.FromImage(newImage))
                    using (Image watermarkImage = Image.FromFile(WatermarkFileChooser.FileName))
                    {
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.DrawImage(srcImage, new Rectangle(0, 0, srcImage.Width, srcImage.Height));
                        graphics.DrawImage(watermarkImage, new Rectangle(new Point((int)logoxaxis.Value, (int)logoyaxis.Value), new Size((int)WatermarkXsize.Value, (int)WatermarkYsize.Value)));
                        if (EnableTextOverlayChk.Checked == true && OverlayText.TextLength > 0)
                        {
                            using (Font textBrush = new Font(textfont.SelectedItem.ToString(), (int)fontSize.Value))
                            {
                                string colorName = textColour.SelectedItem.ToString();

                                int Transparency = TransparencySlider.Value;
                                Color newColor = Color.FromArgb(Transparency, Color.FromName(colorName));
                                SolidBrush brush = new SolidBrush(newColor);

                                PointF Location = new PointF((float)textxaxis.Value, (float)textyaxis.Value);
                                graphics.DrawString(OverlayText.Text.ToString(), textBrush, brush, Location);
                                textBrush.Dispose();
                                brush.Dispose();
                            }
                        }
                        tempImg.SetLength(0);
                        tempImg.Position = 0;
                        newImage.Save(tempImg, System.Drawing.Imaging.ImageFormat.Bmp);
                        ThumbNailPicture.Image = Image.FromStream(tempImg);
                        tempImg.Dispose();
                        srcImage.Dispose();
                        watermarkImage.Dispose();
                        newImage.Dispose();
                    }
                }
            }
            else
            {
                if (EnableTextOverlayChk.Checked == true && OverlayText.TextLength > 0)
                {
                    using (var srcImage = Image.FromStream(tempImg))
                    {
                        using (var newImage = new Bitmap(srcImage.Width, srcImage.Height))
                        using (var graphics = Graphics.FromImage(newImage))
                        {
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            graphics.DrawImage(srcImage, new Rectangle(0, 0, srcImage.Width, srcImage.Height));
                            using (Font textBrush = new Font(textfont.SelectedItem.ToString(), (int)fontSize.Value))
                            {
                                string colorName = textColour.SelectedItem.ToString();

                                int Transparency = TransparencySlider.Value;
                                Color newColor = Color.FromArgb(Transparency, Color.FromName(colorName));
                                SolidBrush brush = new SolidBrush(newColor);

                                PointF Location = new PointF((float)textxaxis.Value, (float)textyaxis.Value);
                                graphics.DrawString(OverlayText.Text.ToString(), textBrush, brush, Location);
                                textBrush.Dispose();
                                brush.Dispose();
                            }

                            tempImg.SetLength(0);
                            tempImg.Position = 0;
                            newImage.Save(tempImg, System.Drawing.Imaging.ImageFormat.Bmp);
                            ThumbNailPicture.Image = Image.FromStream(tempImg);
                            tempImg.Dispose();
                            srcImage.Dispose();
                            newImage.Dispose();
                        }
                    }
                }
            }
        }

        public void SetFontandBrush()
        {

        }

        private void EnableResize_CheckedChanged(object sender, EventArgs e)
        {
            ResizeOptions.Enabled = EnableResize.Checked;
        }

        private void SelectWaterMarkBtn_Click(object sender, EventArgs e)
        {
            WatermarkFileChooser.Filter = "PNG File (.PNG)|*.PNG";
            WatermarkFileChooser.FileName = "";
            DialogResult result = WatermarkFileChooser.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(WatermarkFileChooser.FileName))
            {
                UpdatePreview();
                WatermarkXsize.Enabled = true;
                WatermarkYsize.Enabled = true;
                logoxaxis.Enabled = true;
                logoyaxis.Enabled = true;
            }
        }

        private void EnableWatermarkChkBx_CheckedChanged(object sender, EventArgs e)
        {
            WatermarkOptions.Enabled = EnableWatermarkChkBx.Checked;
        }

        private void WatermarkXsize_ValueChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void WatermarkYsize_ValueChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void logoxaxis_ValueChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void logoyaxis_ValueChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void EnableTextOverlayChk_CheckedChanged(object sender, EventArgs e)
        {
            if (FontsSet == false)
            {
                UpdateFontDropDowns();
                FontsSet = true;
            }
            TextOverlayOptions.Enabled = EnableTextOverlayChk.Checked;
        }

        private void OverlayText_TextChanged(object sender, EventArgs e)
        {
            if (OverlayText.TextLength > 0)
            {
                fontSize.Enabled = true;
                textColour.Enabled = true;
                textfont.Enabled = true;
                textxaxis.Enabled = true;
                textyaxis.Enabled = true;
                TransparencySlider.Enabled = true;
            }
            else
            {
                fontSize.Enabled = false;
                textColour.Enabled = false;
                textfont.Enabled = false;
                textxaxis.Enabled = false;
                textyaxis.Enabled = false;
                TransparencySlider.Enabled = false;
            }
            UpdatePreview();
        }

        private void textColour_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void fontSize_ValueChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void textxaxis_ValueChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void textyaxis_ValueChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void textfont_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        void UpdateFontDropDowns()
        {
            foreach (KnownColor color in Enum.GetValues(typeof(KnownColor)))
            {
                textColour.Items.Add(color.ToString());
            }
            textColour.SelectedIndex = 1;
            foreach (FontFamily font in System.Drawing.FontFamily.Families)
            {
                textfont.Items.Add(font.Name);
            }
            textfont.SelectedIndex = 1;
        }

        private void TransparencySlider_Scroll(object sender, EventArgs e)
        {
            UpdatePreview();
        }
    }
}
