
namespace Test2
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Assert.Equal(3, 5);

        }

        [Fact]
        public void PassingTest()
        {
            Assert.Equal(4, Add(2, 2));
        }

        [Fact]
        public void FailingTest()
        {
            Assert.Equal(5, Add(2, 2));
        }

        int Add(int x, int y)
        {
            return x + y;
        }
    }
}