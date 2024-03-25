using System;
using System.Diagnostics;

namespace APV.Math.MathObjects.Calculations
{
    [DebuggerDisplay("{ToTraceString()}")]
    public abstract class BaseMathCalculationItem : IMathCalculationItem
    {
        private readonly MathOperationContext _context;
        private readonly ulong _id;

        protected BaseMathCalculationItem(MathOperationContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _id = _context.GetNextId();
        }

        public ulong GetId()
        {
            return _id;
        }

        public virtual IMathCalculationContext GetContext()
        {
            return _context;
        }

        public abstract IMathCalculationItem[] GetExecutionList();

        public abstract IMathObject Calculate();

        public abstract MathCalculationItemType GetItemType();

        public abstract string GetMnemonicName();

        public virtual string ToTraceString()
        {
            return string.Format("#{0}: {1}", GetId(), GetMnemonicName());
        }

        public MathOperationContext Context
        {
            get { return _context; }
        }
    }
}