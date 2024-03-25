namespace APV.GraphicsLibrary.Colors.Palettes
{
    public sealed class SpectrumPaletteContainer : BasePaletteContainer
    {
        #region Constructor

        private SpectrumPaletteContainer()
        {
        }

        #endregion

        #region Colors

        public readonly MnemonicColor Red = GlobalPalette.Red;

        public readonly MnemonicColor Orange = GlobalPalette.Orange;

        public readonly MnemonicColor Yellow = GlobalPalette.Yellow;

        public readonly MnemonicColor Green = GlobalPalette.Green;

        public readonly MnemonicColor Cyan = GlobalPalette.Cyan;

        public readonly MnemonicColor Blue = GlobalPalette.Blue;

        public readonly MnemonicColor Violet = GlobalPalette.Violet;

        #endregion

        #region Instance

        public static readonly SpectrumPaletteContainer Instance = new SpectrumPaletteContainer();

        #endregion
    }
}