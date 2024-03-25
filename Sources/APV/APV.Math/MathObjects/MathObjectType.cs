using System;
using APV.Common;

namespace APV.Math.MathObjects
{
    public sealed class MathObjectType : IMathObjectType
    {
        private readonly Type _instanceType;
        private readonly string _mnemonicName;

        public MathObjectType(string mnemonicName, Type instanceType)
        {
            if (mnemonicName == null)
                throw new ArgumentNullException("mnemonicName");
            if (string.IsNullOrWhiteSpace(mnemonicName))
                throw new ArgumentOutOfRangeException("mnemonicName", "Mnemonic name is empty or white space.");
            if (instanceType == null)
                throw new ArgumentNullException("instanceType");
            if (!instanceType.BasedOn<IMathObject>())
                throw new ArgumentOutOfRangeException("instanceType", string.Format("Object type \"{0}\" does not implement interface \"{1}\".", instanceType.FullName, typeof(IMathObject).FullName));
            if (!instanceType.HasDefaultConstructor())
                throw new ArgumentOutOfRangeException("instanceType", string.Format("Object type \"{0}\" does not have default (parameterless).", instanceType.FullName));

            _mnemonicName = mnemonicName;
            _instanceType = instanceType;
        }

        public string GetMnemonicName()
        {
            return _mnemonicName;
        }

        public Type GetInstanceType()
        {
            return _instanceType;
        }

        public IMathObject CreateInstance()
        {
            return (IMathObject) Activator.CreateInstance(_instanceType);
        }

        public IMathObject CreateInstance(string mnemonic)
        {
            IMathObject mathObject = CreateInstance();
            mathObject.FromMnemonic(mnemonic);
            return mathObject;
        }
    }
}