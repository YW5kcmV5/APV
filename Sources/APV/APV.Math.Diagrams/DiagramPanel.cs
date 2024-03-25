using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using APV.Math.Diagrams.Devices;

namespace APV.Math.Diagrams
{
	public class DiagramPanel : Panel
	{
        private readonly SortedList<string, Diagram> _series = new SortedList<string, Diagram>();
	    private readonly SimpleDevice _device;
        private Diagram _mainDiagram;

        public DiagramPanel()
        {
            _device = new SimpleDevice(this);
        }

        public SimpleDevice Device
	    {
            get { return _device; }
	    }

        public Diagram CreateDiagram(string name, string description, float[] x, float[] y, float? minY = null, float? maxY = null, Pen forePen = null)
	    {
	        Diagram diagram;
	        if (_mainDiagram == null)
	        {
	            diagram = new Diagram(x, y, _device, minY, maxY);
                diagram.Resize(Width, Height);
                _mainDiagram = diagram;
	        }
	        else
	        {
	            diagram = new Diagram(x, y, _mainDiagram);
	        }
            diagram.Name = name;
	        diagram.Description = description;
            if (forePen != null)
            {
                diagram.ForePen = forePen;
            }

            return diagram;
	    }

        public Diagram CreateDiagram(string name, string description, float[] x, float[] y, Color foreColor)
        {
            return CreateDiagram(name, description, x, y, null, null, new Pen(foreColor));
        }

        public Diagram CreateDiagram(string series, string name, string description, float[] x, float[] y, float? minY = null, float? maxY = null, Pen forePen = null)
        {
            Diagram diagram;
            int index = _series.IndexOfKey(series);
            if (index == -1)
            {
                diagram = new Diagram(x, y, _device, minY, maxY);
                diagram.Resize(Width, Height);
                _series.Add(series, diagram);
            }
            else
            {
                diagram = new Diagram(x, y, _series.Values[index]);
            }
            diagram.Visible = false;
            diagram.Enabled = false;
            diagram.Name = name;
            diagram.Description = description;
            if (forePen != null)
            {
                diagram.ForePen = forePen;
            }

            return diagram;
        }

        public Diagram CreateDiagram(string series, string name, string description, float[] x, float[] y, Color foreColor)
        {
            return CreateDiagram(series, name, description, x, y, null, null, new Pen(foreColor));
        }

        public bool Contains(string name)
        {
            return (this[name] != null);
        }

	    public Diagram this[string name]
	    {
	        get { return (_mainDiagram != null) ? _mainDiagram.Diagrams[name] : null; }
	    }

	    public Diagram this[string series, string name]
	    {
	        get { return (_series.ContainsKey(series)) ? _series[name].Diagrams[name] : null; }
	    }

	    public void SelectSeries(string series, bool show = false)
	    {
	        Diagram diagramToShow = _series[series];
	        if (!diagramToShow.Visible)
	        {
	            //Hide series
	            foreach (Diagram diagramToHide in _series.Values)
	            {
	                diagramToHide.Visible = false;
	                diagramToHide.Enabled = false;
	            }
	            diagramToShow.Enabled = true;
	            if (show)
	            {
	                diagramToShow.Visible = true;
	                foreach (Diagram child in diagramToShow.Diagrams)
	                {
	                    child.Visible = true;
	                }
                    Invalidate();
                }
	        }
	    }

	    public string[] Names
	    {
	        get { return (_mainDiagram != null) ? _mainDiagram.Diagrams.Names : new string[0]; }
	    }

        public Diagram[] Series
        {
            get { return _series.Values.ToArray(); }
        }

        public string[] SeriesNames
	    {
	        get { return _series.Keys.ToArray(); }
	    }
	}
}