using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using static Load.kqlParser;

namespace Load
{
    class IntegerMathVisitor : kqlBaseVisitor<int>
    {
        public override int VisitCompileUnit(CompileUnitContext context)
        {
            // There can only ever be one expression in a compileUnit. The other node is EOF.
            return Visit(context.expression(0));
        }

        public override int VisitNumber(NumberContext context)
        {
            return int.Parse(context.GetText());
        }

        public override int VisitAddition(AdditionContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            return left + right;
        }

        public override int VisitSubtraction(SubtractionContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            return left - right;
        }

        public override int VisitMultiplication(MultiplicationContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            return left * right;
        }

        public override int VisitDivision(DivisionContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            return left / right;
        }

        private int WalkLeft(ExpressionContext context)
        {
            return Visit(context.GetRuleContext<ExpressionContext>(0));
        }

        private int WalkRight(ExpressionContext context)
        {
            return Visit(context.GetRuleContext<ExpressionContext>(1));
        }
    }
}
