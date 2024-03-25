using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace APV.GraphicsLibrary.Colors.Palettes
{
    public abstract class BasePaletteContainer : MnemonicColors
    {
        private void AddColors()
        {
            FieldInfo[] fields = GetType()
                .GetFields()
                .Where(field => field.FieldType == typeof(MnemonicColor))
                .ToArray();

            MnemonicColor[] colors = fields
                .Select(field => field.GetValue(this) as MnemonicColor)
                .Where(color => color != null)
                .ToArray();

            var list = new SortedList<string, MnemonicColor>();
            foreach (MnemonicColor color in colors)
            {
                if (!list.ContainsKey(color.MnemonicName))
                {
                    list.Add(color.MnemonicName, color);
                }
            }

            AddRange(list.Values);
        }

        private void LoadColors()
        {
            List<FieldInfo> fields = GetType()
                .GetFields()
                .Where(field => field.FieldType == typeof(MnemonicColor))
                .ToList();

            fields.ForEach(field => field.SetValue(this, Get(field.Name)));
        }

        protected BasePaletteContainer()
        {
            AddColors();
            SetReadonly();
        }

        protected BasePaletteContainer(IEnumerable<MnemonicColor> colors)
        {
            AddRange(colors);
            LoadColors();
            SetReadonly();
        }
    }
}