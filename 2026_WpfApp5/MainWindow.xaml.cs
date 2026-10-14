using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _2026_WpfApp5
{
    public partial class MainWindow : Window
    {
        Color strokeColor = Colors.Black;
        Color fillColor = Colors.Transparent;
        int strokeThickness = 1;
        string shapeType = "Line";

        // 繪圖紀錄變數
        Point startPoint;
        Shape currentShape;
        Polyline currentPolyline;
        bool isDrawing = false;

        public MainWindow()
        {
            InitializeComponent();
            StrokeColorPicker.SelectedColor = strokeColor;
            FillColorPicker.SelectedColor = fillColor;
            DisplayStatus();
        }

        private void ShapeButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton targetShapeButton && targetShapeButton.Tag != null)
            {
                shapeType = targetShapeButton.Tag.ToString();
                DisplayStatus();
            }
        }

        private void StrokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                strokeColor = e.NewValue.Value;
                DisplayStatus();
            }
        }

        private void FillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                fillColor = e.NewValue.Value;
                DisplayStatus();
            }
        }

        private void ThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strokeThickness = (int)e.NewValue;
            DisplayStatus();
        }

        private void DisplayStatus()
        {
            if (ColorLabel != null)
                ColorLabel.Content = $"筆刷色彩:{strokeColor}     填充色彩:{fillColor}     筆刷粗細：{strokeThickness}";

            if (ShapeLabel != null)
                ShapeLabel.Content = $"形狀：{shapeType}";
        }

        // ==================== 繪圖核心邏輯 ====================

        // 滑鼠按壓：起點定位與新增圖形
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isDrawing = true;
                startPoint = e.GetPosition(MyCanvas);

                SolidColorBrush strokeBrush = new SolidColorBrush(strokeColor);
                SolidColorBrush fillBrush = new SolidColorBrush(fillColor);

                switch (shapeType)
                {
                    case "Line":
                        currentShape = new Line
                        {
                            X1 = startPoint.X,
                            Y1 = startPoint.Y,
                            X2 = startPoint.X,
                            Y2 = startPoint.Y,
                            Stroke = strokeBrush,
                            StrokeThickness = strokeThickness
                        };
                        break;

                    case "Rectangle":
                        currentShape = new Rectangle
                        {
                            Stroke = strokeBrush,
                            Fill = fillBrush,
                            StrokeThickness = strokeThickness
                        };
                        Canvas.SetLeft(currentShape, startPoint.X);
                        Canvas.SetTop(currentShape, startPoint.Y);
                        break;

                    case "Ellipse":
                        currentShape = new Ellipse
                        {
                            Stroke = strokeBrush,
                            Fill = fillBrush,
                            StrokeThickness = strokeThickness
                        };
                        Canvas.SetLeft(currentShape, startPoint.X);
                        Canvas.SetTop(currentShape, startPoint.Y);
                        break;

                    case "Polyline":
                        currentPolyline = new Polyline
                        {
                            Stroke = strokeBrush,
                            StrokeThickness = strokeThickness
                        };
                        currentPolyline.Points.Add(startPoint);
                        currentShape = currentPolyline;
                        break;
                }

                if (currentShape != null)
                {
                    MyCanvas.Children.Add(currentShape);
                }
            }
        }

        // 滑鼠拖曳：即時更新圖形大小/座標
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && currentShape != null)
            {
                Point currentPoint = e.GetPosition(MyCanvas);

                switch (shapeType)
                {
                    case "Line":
                        var line = currentShape as Line;
                        if (line != null)
                        {
                            line.X2 = currentPoint.X;
                            line.Y2 = currentPoint.Y;
                        }
                        break;

                    case "Rectangle":
                    case "Ellipse":
                        double x = Math.Min(startPoint.X, currentPoint.X);
                        double y = Math.Min(startPoint.Y, currentPoint.Y);
                        double width = Math.Abs(currentPoint.X - startPoint.X);
                        double height = Math.Abs(currentPoint.Y - startPoint.Y);

                        Canvas.SetLeft(currentShape, x);
                        Canvas.SetTop(currentShape, y);
                        currentShape.Width = width;
                        currentShape.Height = height;
                        break;

                    case "Polyline":
                        if (currentPolyline != null)
                        {
                            currentPolyline.Points.Add(currentPoint);
                        }
                        break;
                }
            }
        }

        // 滑鼠放開：結束繪圖
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
            currentShape = null;
            currentPolyline = null;
        }
    }
}