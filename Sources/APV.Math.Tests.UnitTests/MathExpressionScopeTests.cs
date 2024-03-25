using APV.Math.MathObjects.Calculations;
using APV.Math.MathObjects.Calculators;
using APV.Math.MathObjects.Expressions;
using APV.Math.MathObjects.Expressions.Optimizers;
using APV.Math.MathObjects.Numbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Math.Tests.UnitTests
{
    [TestClass]
    public sealed class MathExpressionScopeTests
    {
        [TestMethod]
        public void ParseAndExecutionTest()
        {
            Int64Calculator.Init();

            var testData = new[]
                {
                    new
                        {
                            Expression = @"((x))",
                            OptimizedExpression = @"(x)",
                            Result = (Int64Number)2,
                        },
                    new
                        {
                            Expression = @"x+2*x^2",
                            OptimizedExpression = @"(x+(2*(x^2)))",
                            Result = (Int64Number)10,
                        },
                    new
                        {
                            Expression = @"x+(2*x)+(x^2+3+x)",
                            OptimizedExpression = @"(x+(2*x)+((x^2)+3+x))",
                            Result = (Int64Number)15,
                        },
                    new
                        {
                            Expression = @"x+(2*x)",
                            OptimizedExpression = @"(x+(2*x))",
                            Result = (Int64Number)6,
                        },
                    new
                        {
                            Expression = @"(2*x)+1",
                            OptimizedExpression = @"((2*x)+1)",
                            Result = (Int64Number)5,
                        },
                    new
                        {
                            Expression = @"2*(x+55)+1",
                            OptimizedExpression = @"((2*(x+55))+1)",
                            Result = (Int64Number)115,
                        },
                    new
                        {
                            Expression = @"2*x+55",
                            OptimizedExpression = @"((2*x)+55)",
                            Result = (Int64Number)59,
                        },
                    new
                        {
                            Expression = @"-1^2",
                            OptimizedExpression = @"((-1)^2)",
                            Result = (Int64Number)1,
                        },
                    new
                        {
                            Expression = @"x*x*x+2",
                            OptimizedExpression = @"(((x*x)*x)+2)",
                            Result = (Int64Number)10,
                        },
                    new
                        {
                            Expression = @"x+x+3*x+x-2-x",
                            OptimizedExpression = @"((2*x)+(3*x)-2)",
                            Result = (Int64Number)8,
                        },
                    new
                        {
                            Expression = @"x+x+2*x+x-2-x",
                            OptimizedExpression = @"((2*(2*x))-2)",
                            Result = (Int64Number)6,
                        },
                    new
                        {
                            Expression = @"x*x/2*x*x+4",
                            OptimizedExpression = @"((((x*x)*(x*x))/2)+4)",
                            Result = (Int64Number)12,
                        },
                    new
                        {
                            Expression = @"2*(x+x+3*x+x+x-2-x)+2*(x*x/2*x*x+4)",
                            OptimizedExpression = @"((2*((2*(3*x))-2))+(2*((((x*x)*(x*x))/2)+4)))",
                            Result = (Int64Number)44,
                        }
                };

            foreach (var data in testData)
            {
                string expression = data.Expression;
                string expectedOptimizedExpression = data.OptimizedExpression;
                Int64Number expectedResult = data.Result;

                MathExpressionScope scope = RpnProvider.ParseToExpression(expression);
                scope.Optimize(SimpleMathExpressionOptimizer.Instance);
                MathOperationContext context = RpnProvider.Parse(scope);
                context.SetVariableValue("x", new Int64Number(2));

                string optimizedExpression = scope.GetMnemonicName();
                var result = context.Calculate() as Int64Number;

                string error = string.Format("Expression \"{0}\" can not be parsed and calculated.", expression);

                Assert.IsNotNull(optimizedExpression, error);
                Assert.IsNotNull(result, error);

                Assert.AreEqual(expectedOptimizedExpression, optimizedExpression);
                Assert.AreEqual(expectedResult, result);
            }
        }
    }
}
