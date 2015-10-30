using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DynamicGeometry;
using DynamicGeometry.UI;
using ImageTools;
using ImageTools.IO;
using ImageTools.IO.Bmp;
using ImageTools.IO.Png;
using LiveGeometry.TutorParser;

namespace LiveGeometry
{
    public partial class Page
    {
        private DrawingParserMain parser;
        private BackgroundWorker parseWorker = new BackgroundWorker();
        private ParseOptionsWindow parseOptionsWindow;
        private ProblemCharacteristicsWindow problemCharacteristicsWindow;
        private EnterSolutionWindow enterSolutionWindow;
        private ManageGivensWindow manageGivensWindow;
        private BookProblemWindow bookProblemWindow;
        private SynthesizeProblemWindow synthProblemWindow;
        private GeometryTutorLib.UIDebugPublisher UIDebugPublisher;
        private GeometryTutorLib.UIFigureAnalyzerMain analyzer;
        private GeometryTutorLib.EngineUIBridge.HypergraphWrapper hypergraph;

        private void initParseWorker()
        {
            parseWorker.WorkerReportsProgress = false;
            parseWorker.WorkerSupportsCancellation = false;
            parseWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_ParseToAST);
        }

        void Page_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (Behavior.IsCtrlPressed())
                {
                    if (e.Key == System.Windows.Input.Key.Z)
                    {
                        drawingHost.DrawingControl.CommandUndo.Execute();
                        e.Handled = true;
                        return;
                    }
                    else if (e.Key == Key.Y)
                    {
                        drawingHost.DrawingControl.CommandRedo.Execute();
                        e.Handled = true;
                        return;
                    }
                }

                if (e.Key == Key.F9)
                {
                    PageSettings.Fullscreen();
                    e.Handled = true;
                    return;
                }

                if (drawingHost != null && drawingHost.CurrentDrawing != null && !e.Handled)
                {
                    drawingHost.CurrentDrawing.Behavior.KeyDown(sender, e);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public class ExceptionMessageDialog : MessageBoxDialog
        {
            public ExceptionMessageDialog(Page page, Exception ex, string message)
            {
                parent = page;
                details = new Details(ex.ToString());
                errorCode = ex.ToString().GetHashCode().ToString();
                MessageText = message;
            }

            Page parent;

            [PropertyGridVisible]
            public override string Message
            {
                get
                {
                    return base.Message;
                }
            }

            string errorCode;

            [PropertyGridVisible]
            public string Error
            {
                get
                {
                    return errorCode;
                }
            }

            Details details;
            [PropertyGridVisible]
            [PropertyGridName("Details")]
            public Details ErrorDetails
            {
                get
                {
                    return details;
                }
            }

            public class Details
            {
                public Details(string details)
                {
                    errorDetails = details;
                }

                string errorDetails;

                [PropertyGridVisible]
                [PropertyGridName("Stack trace:")]
                public string ErrorDetails
                {
                    get
                    {
                        return errorDetails;
                    }
                }
            }

            protected override void OKClicked()
            {
                parent.drawingHost.DrawingControl.Drawing.ClearStatus();
            }

            public override string ToString()
            {
                return "Error";
            }
        }

        void HandleException(Exception ex)
        {
            var message =
                "Live Geometry has just encountered an error.\n" +
                "The error details will be reported automatically \n" +
                "and the bug will be fixed as soon as possible.\n\n" +
                "No personal data is transmitted.\n\n" +
                "If you have any questions, please feel free to go to \n" +
                "http://livegeometry.codeplex.com/Thread/List.aspx \n" +
                "and mention the error code from below.\n\n" +
                "Thanks for making Live Geometry better!";

            var dialog = new ExceptionMessageDialog(this, ex, message);
            drawingHost.ShowProperties(dialog);
            if (liveGeometryWebServices == null)
            {
                return;
            }
            try
            {
                liveGeometryWebServices.SendErrorReportAsync(ex.ToString());
            }
            catch (Exception second)
            {
                drawingHost.ShowHint(second.ToString());
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            drawingHost.Clear();
        }

        const string extension = "lgf";
        const string lgfFileFilter = "Live Geometry file (*." + extension + ")|*." + extension;
        const string lgfdgfFileFilter = "Live Geometry file (*.lgf, *.dgf)|*.lgf;*.dgf";
        const string pngFileFilter = "PNG image (*.png)|*.png";
        const string bmpFileFilter = "BMP image (*.bmp)|*.bmp";
        const string dgfFileFilter = "DG Drawing (*.dgf)|*.dgf";
        const string allFileFilter = "All files (*.*)|*.*";
        const string fileFilter = lgfFileFilter
                          + "|" + pngFileFilter
                          + "|" + bmpFileFilter
                          + "|" + allFileFilter
                          ;
        const string openFileFilter = lgfdgfFileFilter
                          + "|" + allFileFilter
                          ;

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }

        private void Open()
        {
            if (Application.Current.HasElevatedPermissions && drawingHost.CurrentDrawing.HasUnsavedChanges)
            {
                ShowUnsavedChangesDialog(ShowOpenFileDialog);
            }
            ShowOpenFileDialog();
        }

        private void ShowOpenFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = openFileFilter;
            dialog.Multiselect = false;
            var result = dialog.ShowDialog();
            if (result != true)
            {
                return;
            }

            string text = null;

            var extension = dialog.File.Extension;
            if (extension.Equals(".dgf", StringComparison.OrdinalIgnoreCase))
            {
                using (var stream = dialog.File.OpenRead())
                {
                    int length = (int)stream.Length;
                    byte[] array = new byte[length];
                    stream.Read(array, 0, length);
                    text = Encoding.FromWindows1251(array);
                    string[] lines = text.Split(
                        new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > 10)
                    {
                        drawingHost.DrawingControl.LoadDrawingFromDGF(lines);
                    }
                    else
                    {
                        drawingHost.ShowHint("Incorrect DGF file (it should contain more than 10 text lines)");
                    }
                }
                return;
            }

            using (var sr = dialog.File.OpenText())
            {
                text = sr.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(text))
            {
                drawingHost.DrawingControl.LoadDrawing(text);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Save()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = fileFilter;
            dialog.FilterIndex = 1;
            dialog.DefaultExt = extension;

            var result = dialog.ShowDialog();
            if (result != true)
            {
                return;
            }

            try
            {
                var fileName = dialog.SafeFileName;
                var actualExtension = fileName.Substring(fileName.LastIndexOf('.')).ToLower();
                if (actualExtension == ".png")
                {
                    SaveAsPng(drawingHost.DrawingControl, dialog);
                }
                else if (actualExtension == ".bmp")
                {
                    SaveAsBmp(drawingHost.DrawingControl, dialog);
                }
                else
                {
                    SaveDrawingToLGF(dialog);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void New()
        {
            if (Application.Current.HasElevatedPermissions && drawingHost.CurrentDrawing.HasUnsavedChanges)
            {
                ShowUnsavedChangesDialog(drawingHost.Clear);
            }
            else
            {
                drawingHost.Clear();
            }
        }

        public void ShowUnsavedChangesDialog(Action subsequentAction)
        {
            var saveButton = new Button()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(8),
                Content = "Save",
                Width = 100
            };
            saveButton.Click += delegate(object sender, RoutedEventArgs e)
            {
                (((sender as Button).Parent as Grid).Parent as ChildWindow).Close();
                Save();
                subsequentAction();
            };

            var dontSaveButton = new Button()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(8),
                Content = "Don't Save",
                Width = 100
            };
            dontSaveButton.Click += delegate(object sender, RoutedEventArgs e)
            {
                (((sender as Button).Parent as Grid).Parent as ChildWindow).Close();
                subsequentAction();
            };

            var cancelButton = new Button()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(8),
                Content = "Cancel",
                Width = 100
            };
            cancelButton.Click += delegate(object sender, RoutedEventArgs e)
            {
                (((sender as Button).Parent as Grid).Parent as ChildWindow).Close();
            };

            TextBlock text = new TextBlock();
            string drawingName = drawingHost.CurrentDrawing.Name;
            if (drawingName == null) drawingName = "untitled";
            text.Text = "Do you want to save changes to \"" + drawingName + "\"?";

            Grid grid = new Grid()
            {
                Width = 360
            };
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Grid.SetRow(text, 0);
            Grid.SetRow(dontSaveButton, 1);
            Grid.SetRow(saveButton, 1);
            Grid.SetRow(cancelButton, 1);
            grid.Children.Add(text, saveButton, dontSaveButton, cancelButton);

            var unsavedChangesWindow = new ChildWindow()
            {
                Content = grid
            };
            unsavedChangesWindow.HasCloseButton = false;
            unsavedChangesWindow.Show();
        }

        void SaveAsPng(Canvas canvas, SaveFileDialog dialog)
        {
            SaveToImage(canvas, dialog, new PngEncoder());
        }

        void SaveAsBmp(Canvas canvas, SaveFileDialog dialog)
        {
            SaveToImage(canvas, dialog, new BmpEncoder());
        }

        void SaveToImage(Canvas canvas, SaveFileDialog dialog, IImageEncoder encoder)
        {
            using (var stream = dialog.OpenFile())
            {
                var image = canvas.ToImage();
                encoder.Encode(image, stream);
            }
        }

        void SaveDrawingToLGF(SaveFileDialog dialog)
        {
            var currentDrawing = drawingHost.CurrentDrawing;
            DrawingSerializer.SaveDrawing(currentDrawing, dialog.OpenFile());
            if (Application.Current.HasElevatedPermissions)
            {
                var undoableActions = currentDrawing.ActionManager.EnumUndoableActions();
                if (!undoableActions.IsEmpty())
                {
                    currentDrawing.LastUndoableActionAtSave = undoableActions.Last();
                }
            }
        }

        /// <summary>
        /// Executed when the parse button is clicked.
        /// Runs the background parse thread.
        /// </summary>
        void ParseToAst()
        {
            if (!parseWorker.IsBusy)
            {
                parser = new DrawingParserMain(drawingHost.CurrentDrawing);
                //Do parse and back-end computation on background worker
                parseWorker.RunWorkerAsync();
            }
            else
            {
                UIDebugPublisher.publishString("Process Busy: Please wait for completion before starting a new parse.");
            }
        }

        /// <summary>
        /// Runs the Parse and back-end computations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BackgroundWorker_ParseToAST(object sender, DoWorkEventArgs e)
        {
            UIDebugPublisher.clearWindow();
            UIDebugPublisher.publishString("Starting Parse Process...");

            // Execute Front-End Parse
            parser.Parse();

            foreach (GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion ar in parser.backendParser.GetAtomicRegions())
            {
                UIDebugPublisher.publishString(ar.ToString());
            }

            analyzer = new GeometryTutorLib.UIFigureAnalyzerMain(parser.backendParser.MakeProblemDescription(manageGivensWindow.GetGivens()));
            List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> problems = analyzer.AnalyzeFigure();

            // Acquire access to the backend hypergraph
            hypergraph = analyzer.GetHypergraphWrapper();

            foreach (GeometryTutorLib.ConcreteAST.GroundedClause gc in manageGivensWindow.GetGivens())
            {
                UIDebugPublisher.publishString("Given: " + gc.ToString());
            }

            //Example of UI Output to AI Window
            foreach (GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation> problem in problems)
            {
                UIDebugPublisher.publishString(problem.ConstructProblemAndSolution(analyzer.graph).ToString());
            }

            enterSolutionWindow.problem = problems[0];

            UIDebugPublisher.publishString("Parse Complete.");
        }

        void DisplayParseOptions()
        {
            parseOptionsWindow.Show();
        }

        void ParseOptionsWindow_Closed(object sender, EventArgs e)
        {
            if (isolatedSettings.Contains("UserParseGroup"))
            {
                isolatedSettings["UserParseGroup"] = ParseGroupWindow.GetUserGroups();
            }
            else
            {
                isolatedSettings.Add("UserParseGroup", ParseGroupWindow.GetUserGroups());
            }
        }

        void DisplayProblemCharacteristics()
        {
            problemCharacteristicsWindow.Show();
        }

        void ProblemCharacteristicsWindow_Closed(object sender, EventArgs e)
        {
            //Do whatever needs to be done when the problem characteristics window closes
        }

        void DisplayManageGivens()
        {
            problemCharacteristicsWindow.ShowManageGivensWindow();
        }

        void DisplayEnterSolution()
        {
            if (enterSolutionWindow.problem != null)
            {
                enterSolutionWindow.Show();
            }
            else
            {
                MessageBox.Show("Please enter a valid problem before entering a solution.",
                    "Cannot Enter Solution",
                    MessageBoxButton.OK);
            }
        }

        void EnterSolutionWindow_Closed(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        void StartRegionShading()
        {
            if (!ShadingMode)
            {
                UpdateShadingMode(true);

                //Parse and set atoms
                DrawingParserMain parser = new DrawingParserMain(drawingHost.CurrentDrawing);
                parser.Parse();
                DynamicGeometry.UI.RegionShading.ShadedRegionCreator.Atoms = parser.backendParser.GetAtomicRegions();
            }
        }

        void ClearRegionShading()
        {
            if (ShadingMode)
            {
                UpdateShadingMode(false);

                //Discard regions
                drawingHost.CurrentDrawing.ClearRegionShadings();
            }
        }

        void DrawBookProblem()
        {
            if (!ShadingMode)
            {
                bookProblemWindow.Show();
                UpdateShadingMode(true);
            }
        }

        void SynthesizeProblem()
        {
            if (!ShadingMode)
            {
                synthProblemWindow.Show();
                UpdateShadingMode(true);
            }
        }

        void UpdateShadingMode(bool mode)
        {
            ShadingMode = mode;

            //Toggle behaviors
            Behaviors.ForEach<Behavior>(b => b.Enabled = !b.Enabled);

            //Graphical Update
            if (mode)
            {
                //Enable
                CommandClearRegionShading.Icon.Opacity = 1.0;

                //Disable
                CommandStartRegionShading.Icon.Opacity = 0.2;
                CommandMakeBookProblem.Icon.Opacity = 0.2;
                CommandSynthProblem.Icon.Opacity = 0.2;
       
            }
            else
            {
                //Enable
                CommandStartRegionShading.Icon.Opacity = 1.0;
                CommandMakeBookProblem.Icon.Opacity = 1.0;
                CommandSynthProblem.Icon.Opacity = 1.0;

                //Disable
                CommandClearRegionShading.Icon.Opacity = 0.2;
            }
        }

        void DrawProblemWindow_Close(object sender, EventArgs e)
        {
            var regions = drawingHost.CurrentDrawing.GetRegionShadings();
            var atomic = new List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion>();
            regions.ForEach(r => atomic.Add(r.Region));
            DynamicGeometry.UI.RegionShading.ShadedRegionCreator.Atoms = atomic;
        }
    }
}