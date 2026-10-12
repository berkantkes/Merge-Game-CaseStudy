using MergeGame.Items;

namespace MergeGame.Game
{
    public interface IMergeScoreCalculator
    {
        int Calculate(ItemStack mergedItem);
    }
}
