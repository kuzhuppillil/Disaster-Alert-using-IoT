using DisasterAlertNode.Core.Serial.AtCommands;

namespace DisasterAlertNode.Core.Serial.Configurator
{
    internal sealed class BleCommandReader : CommandReader
    {
        sealed public override void RetrieveCommands()
        {
            // Add all command and responses needed for configuring the BLE module.
            Command command = new AtCommand("AT");
            _commands.Add(command);

            //command = new NotificationCommand("AT+NOTI1");
            //_commands.Add(command);

            command = new AddressCommand("AT+ADDR?");
            _commands.Add(command);

            command = new NameCommand("AT+NAME?");
            _commands.Add(command);

            command = new FilterCommand("AT+FILT1");
            _commands.Add(command);

            command = new ShowCommand("AT+SHOW1");
            _commands.Add(command);


            command = new RoleCommand("AT+ROLE1");
            _commands.Add(command);

            command = new IMMECommand("AT+IMME1");
            _commands.Add(command);

            command = new DiscoveryCommand("AT+DISC?");
            _commands.Add(command);

            command = new ConnectCommand("AT+CON{0}");
            _commands.Add(command);

            command = new StartCommand("AT+START");
            _commands.Add(command);


        }
    }
}
