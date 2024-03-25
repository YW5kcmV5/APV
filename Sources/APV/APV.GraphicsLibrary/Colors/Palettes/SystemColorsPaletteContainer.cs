namespace APV.GraphicsLibrary.Colors.Palettes
{
    public sealed class SystemColorsPaletteContainer : BasePaletteContainer
    {
        #region Constructor

        private SystemColorsPaletteContainer()
        {
        }

        #endregion

        #region Colors

        public readonly MnemonicColor ActiveBorder = GlobalPalette.ActiveBorder;

        public readonly MnemonicColor ActiveCaption = GlobalPalette.ActiveCaption;

        public readonly MnemonicColor ActiveCaptionText = GlobalPalette.White;

        public readonly MnemonicColor AppWorkspace = GlobalPalette.Gray;

        public readonly MnemonicColor ButtonFace = GlobalPalette.ActiveBorder;

        public readonly MnemonicColor ButtonHighlight = GlobalPalette.White;

        public readonly MnemonicColor ButtonShadow = GlobalPalette.Gray;

        public readonly MnemonicColor Control = GlobalPalette.ActiveBorder;

        public readonly MnemonicColor ControlDark = GlobalPalette.Gray;

        public readonly MnemonicColor ControlDarkDark = GlobalPalette.ControlDarkDark;

        public readonly MnemonicColor ControlLight = GlobalPalette.ActiveBorder;

        public readonly MnemonicColor ControlLightLight = GlobalPalette.White;

        public readonly MnemonicColor ControlText = GlobalPalette.Black;

        public readonly MnemonicColor Desktop = GlobalPalette.Desktop;

        public readonly MnemonicColor GradientActiveCaption = GlobalPalette.GradientActiveCaption;

        public readonly MnemonicColor GradientInactiveCaption = GlobalPalette.Silver;

        public readonly MnemonicColor GrayText = GlobalPalette.Gray;

        public readonly MnemonicColor Highlight = GlobalPalette.ActiveCaption;

        public readonly MnemonicColor HighlightText = GlobalPalette.White;

        public readonly MnemonicColor HotTrack = GlobalPalette.Navy;

        public readonly MnemonicColor InactiveBorder = GlobalPalette.ActiveBorder;

        public readonly MnemonicColor InactiveCaption = GlobalPalette.Gray;

        public readonly MnemonicColor InactiveCaptionText = GlobalPalette.ActiveBorder;

        public readonly MnemonicColor Info = GlobalPalette.Info;

        public readonly MnemonicColor InfoText = GlobalPalette.Black;

        public readonly MnemonicColor Menu = GlobalPalette.ActiveBorder;

        public readonly MnemonicColor MenuBar = GlobalPalette.ActiveBorder;

        public readonly MnemonicColor MenuHighlight = GlobalPalette.ActiveCaption;

        public readonly MnemonicColor MenuText = GlobalPalette.Black;

        public readonly MnemonicColor ScrollBar = GlobalPalette.ActiveBorder;

        public readonly MnemonicColor Window = GlobalPalette.White;

        public readonly MnemonicColor WindowFrame = GlobalPalette.Black;

        public readonly MnemonicColor WindowText = GlobalPalette.Black;

        #endregion

        #region Instance

        public static readonly SystemColorsPaletteContainer Instance = new SystemColorsPaletteContainer();

        #endregion
    }
}