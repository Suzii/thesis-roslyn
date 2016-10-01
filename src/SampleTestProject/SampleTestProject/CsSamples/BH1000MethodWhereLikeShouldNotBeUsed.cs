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
    }
}
