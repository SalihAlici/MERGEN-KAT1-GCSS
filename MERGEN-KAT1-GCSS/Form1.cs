using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.WinForms;
using LiveCharts.Wpf;

namespace MERGEN_KAT1_GCSS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string imagePath = @"C:\Users\deniz\Downloads\mergen.png";

            // Resmi yükle
            Bitmap bitmap = new Bitmap(imagePath);

            // Belirli bir noktadaki rengi al (Örneğin, resmin ortası)
            int x = bitmap.Width / 2;
            int y = bitmap.Height / 15;
            Color selectedColor = bitmap.GetPixel(x, y);

            // Form'un arka plan rengini değiştir
            tableLayoutPanel5.BackColor = selectedColor;





            // İlk grafik (cartesianChart1)
            cartesianChart1.Series = new SeriesCollection
            {
                // 1. Kırmızı Çizgi
                new LineSeries
                {
                    Title = "Kırmızı Grafik",
                    Values = new ChartValues<double> { 3, 5, 7, 4,78,9 },
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red), // Kırmızı çizgi
                    PointGeometry = null, // Noktaları kaldır
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(150, 255, 0, 0)) // Kırmızı dolgu
                },
                new LineSeries
                {
                    Title = "Kahverengi Grafik",
                    Values = new ChartValues<double> { 6,5,78,8,7,2 }, // Tek veri noktası
                    PointGeometry = null, // Noktaları kaldır
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.BlueViolet), // Kırmızı çizgi
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(150, 75, 0, 250)) // Kahverengi dolgu (şeffaf)
                }

            };

            // Başlıkları üstte görmek için
            cartesianChart1.LegendLocation = LegendLocation.Top; // Üstte başlıkları gör

            // Yeni grafik (cartesianChart2) - Kahverengi Tek Veri Noktalı Grafik
            cartesianChart2.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Kırmızı Grafik",
                    Values = new ChartValues<double> { 3, 5, 7, 4,78,9 },
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red), // Kırmızı çizgi
                    PointGeometry = null, // Noktaları kaldır
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(150, 255, 0, 0)) // Kırmızı dolgu
                },
                new LineSeries
                {
                    Title = "Kahverengi Grafik",
                    Values = new ChartValues<double> { 6,5,78,8,7,2 }, // Tek veri noktası
                    PointGeometry = null, // Noktaları kaldır
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.BlueViolet), // Kırmızı çizgi
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(150, 75, 0, 250)) // Kahverengi dolgu (şeffaf)
                }
            };

            // Başlıkları üstte görmek için
            cartesianChart2.LegendLocation = LegendLocation.Top;


            // İlk grafik (cartesianChart1)
            cartesianChart3.Series = new SeriesCollection
            {
                // 1. Kırmızı Çizgi
                new LineSeries
                {
                    Title = "Kırmızı Grafik",
                    Values = new ChartValues<double> { 3, 5, 7, 4,78,9 },
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red), // Kırmızı çizgi
                    PointGeometry = null, // Noktaları kaldır
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(150, 255, 0, 0)) // Kırmızı dolgu
                },
                new LineSeries
                {
                    Title = "Kahverengi Grafik",
                    Values = new ChartValues<double> { 6,5,78,8,7,2 }, // Tek veri noktası
                    PointGeometry = null, // Noktaları kaldır
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.BlueViolet), // Kırmızı çizgi
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(150, 75, 0, 250)) // Kahverengi dolgu (şeffaf)
                } };
            cartesianChart3.LegendLocation = LegendLocation.Top;



            // İlk grafik (cartesianChart1)
            cartesianChart4.Series = new SeriesCollection
            {
                // 1. Kırmızı Çizgi
                new LineSeries
                {
                    Title = "Kırmızı Grafik",
                    Values = new ChartValues<double> { 3, 5, 7, 4,78,9 },
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red), // Kırmızı çizgi
                    PointGeometry = null, // Noktaları kaldır
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(150, 255, 0, 0)) // Kırmızı dolgu
                },
                new LineSeries
                {
                    Title = "Kahverengi Grafik",
                    Values = new ChartValues<double> { 6,5,78,8,7,2 }, // Tek veri noktası
                    PointGeometry = null, // Noktaları kaldır
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.BlueViolet), // Kırmızı çizgi
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(150, 75, 0, 250)) // Kahverengi dolgu (şeffaf)
                } };
            cartesianChart4.LegendLocation = LegendLocation.Top;



            // İlk grafik (cartesianChart1)
            cartesianChart5.Series = new SeriesCollection
            {
                // 1. Kırmızı Çizgi
                new LineSeries
                {
                    Title = "Kırmızı Grafik",
                    Values = new ChartValues<double> { 3, 5, 7, 4,78,9 },
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red), // Kırmızı çizgi
                    PointGeometry = null, // Noktaları kaldır
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(150, 255, 0, 0)) // Kırmızı dolgu
                },
                new LineSeries
                {
                    Title = "Kahverengi Grafik",
                    Values = new ChartValues<double> { 6,5,78,8,7,2 }, // Tek veri noktası
                    PointGeometry = null, // Noktaları kaldır
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.BlueViolet), // Kırmızı çizgi
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(150, 75, 0, 250)) // Kahverengi dolgu (şeffaf)
                } };
            cartesianChart5.LegendLocation = LegendLocation.Top;

            cartesianChart6.Series = new SeriesCollection
            {
                // 1. Kırmızı Çizgi
                new LineSeries
                {
                    Title = "Kırmızı Grafik",
                    Values = new ChartValues<double> { 3, 5, 7, 4,78,9 },
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red), // Kırmızı çizgi
                    PointGeometry = null, // Noktaları kaldır
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(150, 255, 0, 0)) // Kırmızı dolgu
                },
                new LineSeries
                {
                    Title = "Kahverengi Grafik",
                    Values = new ChartValues<double> { 6,5,78,8,7,2 }, // Tek veri noktası
                    PointGeometry = null, // Noktaları kaldır
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.BlueViolet), // Kırmızı çizgi
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(150, 75, 0, 250)) // Kahverengi dolgu (şeffaf)
                } };
            cartesianChart6.LegendLocation = LegendLocation.Top;

        }

       
    };
        }
    

