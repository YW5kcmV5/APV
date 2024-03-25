using System;

namespace APV.Math.Diagrams
{
    public enum DiagramEventType
    {
        HintSelectedPoint,
        ShowSelectedPoint,
        HideSelectedPoint,
        MouseMove,
    }

    public abstract class DiagramEventArgs : EventArgs
    {
        public DiagramEventType EventType { get; protected set; }

        public bool Handled { get; set; }
    }

    public abstract class SelectedPointEventArgs : DiagramEventArgs
    {
        public DiagramPoint Point { get; set; }

        public string Hint { get; set; }
    }

    public class HintSelectedPointEventArgs : SelectedPointEventArgs
    {
        public HintSelectedPointEventArgs()
        {
            EventType = DiagramEventType.HintSelectedPoint;
        }
    }

    public class ShowSelectedPointEventArgs : SelectedPointEventArgs
    {
        public ShowSelectedPointEventArgs()
        {
            EventType = DiagramEventType.ShowSelectedPoint;
        }
    }

    public class HideSelectedPointEventArgs : SelectedPointEventArgs
    {
        public HideSelectedPointEventArgs()
        {
            EventType = DiagramEventType.ShowSelectedPoint;
        }
    }

    public class MouseMoveEventArgs : DiagramEventArgs
    {
        public DiagramPoint CurrentPoint { get; set; }

        public MouseMoveEventArgs()
        {
            EventType = DiagramEventType.MouseMove;
        }
    }

    public delegate void DiagramEventDelegate(Diagram diagram, DiagramEventArgs ergs);
}
