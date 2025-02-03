using System.Collections.Generic;

namespace SequenceSDK.Samples
{
    public struct DailyRewardsStatusData
    {
        public int nextClaimTime;
        public UserRewardStatusData userStatus;
        public List<List<RewardData>> rewards;
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
