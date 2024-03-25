namespace APV.GraphicsLibrary.Colors.Palettes
{
    /// <summary>
    /// Source: http://www.w3.org/TR/REC-html40/types.html#h-6.5
    /// </summary>
    public sealed class HtmlPaletteContainer : BasePaletteContainer
    {
        #region Constructor

        private HtmlPaletteContainer()
        {
        }

        #endregion

        #region Colors

        public readonly MnemonicColor Black = GlobalPalette.Black;

        public readonly MnemonicColor Silver = GlobalPalette.Silver;

        public readonly MnemonicColor Gray = GlobalPalette.Gray;

        public readonly MnemonicColor White = GlobalPalette.White;

        public readonly MnemonicColor Maroon = GlobalPalette.Maroon;

        public readonly MnemonicColor Red = GlobalPalette.Red;

        public readonly MnemonicColor Purple = GlobalPalette.Purple;

        public readonly MnemonicColor Fuchsia = GlobalPalette.Fuchsia;

        public readonly MnemonicColor Green = GlobalPalette.GreenHtml;

        public readonly MnemonicColor Lime = GlobalPalette.Green;

        public readonly MnemonicColor Olive = GlobalPalette.Olive;

        public readonly MnemonicColor Yellow = GlobalPalette.Yellow;

        public readonly MnemonicColor Navy = GlobalPalette.Navy;

        public readonly MnemonicColor Blue = GlobalPalette.Blue;

        public readonly MnemonicColor Teal = GlobalPalette.Teal;

        public readonly MnemonicColor Aqua = GlobalPalette.Cyan;

        #endregion

        #region Instance

        public static readonly HtmlPaletteContainer Instance = new HtmlPaletteContainer();

        #endregion
    }
}