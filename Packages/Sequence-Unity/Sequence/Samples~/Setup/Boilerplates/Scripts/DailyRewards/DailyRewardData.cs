using System.Collections.Generic;

namespace Sequence.Boilerplates.DailyRewards
{
    public struct DailyRewardsStatusData
    {
        public int timeSpan;
        public UserRewardStatusData userStatus;
        public List<RewardData> rewards;
    }

    public struct UserRewardStatusData
    {
        public string id;
        public int progress;
        public int lastClaimTime;
    }

    public struct RewardData
    {
        public string name;
        public string type;
        public string contractAddress;
        public int tokenId;
        public int amount;
    }
}
