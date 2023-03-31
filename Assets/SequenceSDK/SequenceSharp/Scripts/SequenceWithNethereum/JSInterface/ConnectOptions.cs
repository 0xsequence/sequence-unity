namespace SequenceSharp
{
    public class ConnectOptions
    {
#nullable enable
        /**
            <summary>
            Specifies the default network a dapp would like to connect to. This field
            is optional as it can be provided a number of different ways.
            </summary>
        */
        public string? networkId;

        /**
            <summary>
            App name of the dApp; Will be announced to user on connect screen.
            </summary>
        */
        public string? app;

        /**
            <summary>
            Social auth redirect protocol - usually set internally by the Wallet.
            </summary>
        */
        public string? appProtocol;

        /**
            <summary>
            Expiry number (in seconds) to expire connect session. Default is 1 week of seconds.
            </summary>
        */
        public ulong? expiry;

        /**
            <summary>
            Perform an ETHAuth eip712 signing and return the proof to the dApp.
            </summary>
        */
        public bool? authorize;

        /**
            <summary>
            Force a full re-connect (ie. disconnect then connect again)
            </summary>
        */
        public bool? refresh;

        /**
            <summary>
            KeepWalletOpened will keep the wallet window opened after connecting.
            The default is to automatically close the wallet after connecting.
            </summary>
        */
        public bool? keepWalletOpened;
#nullable disable

        
    }
}