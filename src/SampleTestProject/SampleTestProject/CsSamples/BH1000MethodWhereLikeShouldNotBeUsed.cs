using System;
using CMS.DataEngine;

namespace SampleTestProject.CsSamples
{
    public class BH1000MethodWhereLikeShouldNotBeUsed
    {
        public void WhereLike() {
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.WhereLike("columnName", "value");

        }

        public void FalsePositiveForWhereLike()
        {
            WhereLike();
            this.WhereLike();
        }

        public void WhereNotLike()
        {
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.WhereNotLike("columnName", "value");
        }

        public void FalsePositiveForWhereNotLike()
        {
            this.WhereNotLike();
            WhereNotLike();
        }

        public void MethodWithDelegateAsParam(Func<string, string, WhereCondition> func)
        {
            var whereCondition = func("columnName", "value");
        }

        public void MethodPassingWhereLikeAsDelegate()
        {
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.WhereLike("columnName", "value");

            MethodWithDelegateAsParam(whereCondition.WhereLike);
        }
    }
}
