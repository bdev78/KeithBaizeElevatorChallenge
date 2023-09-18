using KeithBaizeElevatorChallenge.Models;

namespace KeithBaizeElevatorChallenge {
    class Program {
        static async Task Main(string[] args) {
            Elevator elevator = new Elevator(5);
            int genericDelay = 2000;
            var cts = new CancellationTokenSource();
            Task elevatorTask = elevator.ProcessRequestsAsync(cts.Token);

            int[] elevatorCalls = { 3, 2 };
            int[] validInsideButtonsOne = { 2, 5, 6 };
            int[] validInsideButtonsTwo = { 1, 7, 4, 9 };
            int[] invalidInsideButtons = { 99999, -256, -512 };

            foreach (var originFloor in elevatorCalls) {
                elevator.CallElevator(originFloor);
                await Task.Delay(genericDelay);
            }
            foreach (var targetFloor in validInsideButtonsOne) {
                elevator.PressButtonInsideElevator(targetFloor);
            }
            await Task.Delay(genericDelay);
            foreach (var targetFloor in validInsideButtonsTwo) {
                elevator.PressButtonInsideElevator(targetFloor);
            }
            await Task.Delay(20000);
            foreach (var targetFloor in invalidInsideButtons) {
                elevator.PressButtonInsideElevator(targetFloor);
            }

            await Task.Delay(20000);
            cts.Cancel();
            try {
                await elevatorTask;
            }
            catch (OperationCanceledException) {
                Console.WriteLine("Elevator task was canceled.");
            }

            Console.WriteLine("Execution complete!");
        }
    }
}
