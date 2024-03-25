using System;
using APV.Common;
using APV.Math.MathObjects.Calculations;
using APV.Math.MathObjects.Calculators;
using APV.Math.MathObjects.Expressions;
using APV.Math.MathObjects.Expressions.Optimizers;
using APV.Math.MathObjects.Functions;
using APV.Math.MathObjects.Numbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Math.Tests.ConsoleTest
{
    public static class MathCalculationTests
    {
        private static void ScopesTest1()
        {
            const string expression = @"2*(x+x+3*x+x+x-2-x)+2*(x*x/2*x*x+4)";
            //const string expression = @"x+2*x^2";
            //const string expression = @"x+(2*x)+(x^2+3+x)";
            //const string expression = @"x+(2*x)";
            //const string expression = @"(2*x)+1";
            //const string expression = @"2*(x+55)+1";
            //const string expression = @"2*x+55";
            //const string expression = @"-1^-2";
            //const string expression = @"x*x*x+2";
            //const string expression = @"x*x/2*x*x+4";
            //const string expression = @"((x))";
            //const string expression = @"x+x+3*x+x-2-x";
            //const string expression = @"3/7*x*x/5*x";
            //const string expression = @"3/7";
            //const string expression = @"3*4/7";
            //const string expression = @"3*4/7*5/2*3/3/3";
            //const string expression = @"3*4/7*5/3";
            //const string expression = @"3/7/3";
            //const string expression = @"x*x/2*x*x+4";
            //const string expression = @"x*x*2*3*x*4*x*x";
            //const string expression = @"x*x*2*3*x*4*x*x*x*x*2*3*x*4*x*x";
            //const string expression = @"x*y*x*x+2";

            MathExpressionScope scope = RpnProvider.ParseToExpression(expression);
            scope.Optimize(SimpleMathExpressionOptimizer.Instance);
            MathOperationContext context = RpnProvider.Parse(scope);
            context.SetVariableValue("x", new Int64Number(3));

            Assert.IsNotNull(context);

            IMathCalculationItem[] executionList = context.GetExecutionList();

            for (int i = 0; i < executionList.Length; i++)
            {
                var item = (BaseMathCalculationItem)executionList[i];
                Console.WriteLine(item.ToTraceString());
            }
        }

        private static void CalculateTest1()
        {
            var x = new Int64Number(2);
            var y = new Int64Number(3);
            var z = new Int64Number(4);

            Assert.IsNotNull(Int64Calculator.Instance);

            IMathObject r0 = MathFunctionType.Multiplication.Calculate(x, y);
            IMathObject ret = MathFunctionType.Addition.Calculate(r0, z);

            Assert.AreEqual("10", ret.ToMnemonic());

            Console.WriteLine(ret.ToMnemonic());
        }

        private static void CalculateTest2()
        {
            var context = new MathOperationContext();
            var vx = new MathArgumentItem(context, "2");
            var vy = new MathArgumentItem(context, "3");
            var vz = new MathArgumentItem(context, "4");
            var step1 = new MathOperationItem(context, MathFunctionType.Multiplication, vx, vy);
            var step2 = new MathOperationItem(context, MathFunctionType.Addition, step1, vz);
            context.SetEntryPoint(step2);

            IMathObject ret = context.Calculate();

            Assert.AreEqual("10", ret.ToMnemonic());

            Console.WriteLine(ret.ToMnemonic());
        }

        private static void CalculateTest3()
        {
            const string expression = @"x*y+4";

            Assert.IsNotNull(MathFunctionType.Multiplication);
            Assert.IsNotNull(MathFunctionType.Addition);

            MathOperationContext context = RpnProvider.Parse(expression);

            var x = new Int64Number(2);
            var y = new Int64Number(3);

            context.SetVariableValue("x", x);
            context.SetVariableValue("y", y);

            IMathObject ret = context.Calculate();

            Assert.AreEqual("10", ret.ToMnemonic());

            Console.WriteLine(ret.ToMnemonic());
        }

        private static void GetExecutionListTest1()
        {
            Console.WriteLine();
            Console.WriteLine("GetExecutionListTest.Start()");
            Console.WriteLine();

            const string expression = @"x*((x*y+4)+(25*x+3))+(x*y)+x^3+x^y";

            MathOperationContext context = RpnProvider.Parse(expression);

            IMathCalculationItem[] executionList = context.GetExecutionList();

            for (int i = 0; i < executionList.Length; i++)
            {
                var item = (BaseMathCalculationItem) executionList[i];
                Console.WriteLine(item.ToTraceString());
            }

            Console.WriteLine();
            Console.WriteLine("GetExecutionListTest.End()");
        }

        private static void GetExecutionListTest2()
        {
            Console.WriteLine();
            Console.WriteLine("GetExecutionListTest.Start()");
            Console.WriteLine();

            const string expression = @"x*x*x*x*x*x*x*x";

            MathOperationContext context = RpnProvider.Parse(expression);

            IMathCalculationItem[] executionList = context.GetExecutionList();

            for (int i = 0; i < executionList.Length; i++)
            {
                var item = (BaseMathCalculationItem)executionList[i];
                Console.WriteLine(item.ToTraceString());
            }

            Console.WriteLine();
            Console.WriteLine("GetExecutionListTest.End()");
        }

        public static void Execute()
        {
            try
            {
                ScopesTest1();
                CalculateTest1();
                CalculateTest2();
                CalculateTest3();
                GetExecutionListTest1();
                GetExecutionListTest2();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.");
                Console.WriteLine("Error={0}", ex.ToTraceString());
                throw;
            }
        }
    }
}
