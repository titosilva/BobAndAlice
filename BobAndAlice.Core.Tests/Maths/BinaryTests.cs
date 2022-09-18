using BobAndAlice.Core.Maths;
using Xunit;

namespace BobAndAlice.Core.Tests.Maths
{
    public class BinaryTests
    {
        [Fact]
        public void ToWords__ShouldReturnVector__WithAllBytes()
        {
            var prng = new Prng();
            var randomNumber = prng.Next(19).ToBinary();
            var randomNumberWords = randomNumber.ToWords();

            Assert.Equal(5, randomNumberWords.Count);
            var wordIdx = 0;
            foreach (var word in randomNumberWords)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (randomNumber.Length <= wordIdx * 4 + i)
                    {
                        break;
                    }

                    Assert.Equal(randomNumber.Content[wordIdx * 4 + i], (word >> (i * 8)) & 0xff);
                }

                wordIdx++;
            }
        }

        [Fact]
        public void BinaryFromWordsToWords__ShouldKeepSameOriginalWords()
        {
            var prng = new Prng();
            var binaryTest = prng.Next(19).ToBinary();
            var words = binaryTest.ToWords();
            var binaryCopy = new Binary(words);
            var binaryCopyWords = binaryCopy.ToWords();

            Assert.Equal(words, binaryCopyWords);
        }
    }
}