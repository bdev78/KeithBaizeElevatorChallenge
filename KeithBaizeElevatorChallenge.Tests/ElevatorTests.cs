using KeithBaizeElevatorChallenge.Models;
using Xunit;

namespace KeithBaizeElevatorChallenge.Tests {
    public class ElevatorTests {
        [Fact]
        public async Task MoveToFloorAsync_SuccessfulWhenTargetFloorIsValid() {
            // Arrange
            int startingFloor = 1;
            int targetFloor = 5;
            int requestDelay = 2000;
            var cts = new CancellationTokenSource();
            Elevator elevator = new Elevator(startingFloor);
            Task elevatorTask = elevator.ProcessRequestsAsync(cts.Token);

            // Act
            await elevator.MoveToFloorAsync(targetFloor);
            await Task.Delay(requestDelay);
            cts.Cancel();
            try {
                await elevatorTask;
            }
            catch (OperationCanceledException) {
                Console.WriteLine("Elevator task was canceled.");
            }

            // Assert
            Assert.True(elevatorTask.IsCompleted);
            Assert.Equal(targetFloor, elevator.currentFloor);
        }

        [Fact]
        public async Task MoveToFloorAsync_FailWhenTargetFloorBelowMinimumLimit() {
            // Arrange
            int startingFloor = 1;
            int targetFloor = -1;
            int requestDelay = 2000;
            var cts = new CancellationTokenSource();
            Elevator elevator = new Elevator(startingFloor);
            Task elevatorTask = elevator.ProcessRequestsAsync(cts.Token);

            // Act
            await elevator.MoveToFloorAsync(targetFloor);
            await Task.Delay(requestDelay);
            cts.Cancel();
            try {
                await elevatorTask;
            }
            catch (OperationCanceledException) {
                Console.WriteLine("Elevator task was canceled.");
            }

            // Assert
            Assert.True(elevatorTask.IsCompleted);
            Assert.NotEqual(targetFloor, elevator.currentFloor);
            Assert.Equal(startingFloor, elevator.currentFloor);
        }

        [Fact]
        public async Task MoveToFloorAsync_FailWhenTargetFloorAboveMaximumLimit() {
            // Arrange
            int startingFloor = 1;
            int targetFloor = 1337;
            int requestDelay = 2000;
            var cts = new CancellationTokenSource();
            Elevator elevator = new Elevator(startingFloor);
            Task elevatorTask = elevator.ProcessRequestsAsync(cts.Token);

            // Act
            await elevator.MoveToFloorAsync(targetFloor);
            await Task.Delay(requestDelay);
            cts.Cancel();
            try {
                await elevatorTask;
            }
            catch (OperationCanceledException) {
                Console.WriteLine("Elevator task was canceled.");
            }

            // Assert
            Assert.True(elevatorTask.IsCompleted);
            Assert.NotEqual(targetFloor, elevator.currentFloor);
            Assert.Equal(startingFloor, elevator.currentFloor);
        }

        [Fact]
        public async Task ProcessRequestsAsync_MultipleValidFloorRequests() {
            // Arrange
            int startingFloor = 1;
            int[] targetFloors = new int[] { 5, 3, 8 };
            int requestDelay = 9001;
            var cts = new CancellationTokenSource();
            Elevator elevator = new Elevator(startingFloor);
            Task elevatorTask = elevator.ProcessRequestsAsync(cts.Token);

            // Act
            foreach (var floor in targetFloors) {
                elevator.CallElevator(floor);
            }
            await Task.Delay(requestDelay);

            cts.Cancel();

            try {
                await elevatorTask;
            }
            catch (OperationCanceledException) {
                Console.WriteLine("Elevator task was canceled.");
            }

            // Assert
            Assert.True(elevatorTask.IsCompleted);
            Assert.Contains(elevator.currentFloor, targetFloors);
        }

        [Fact]
        public async Task ProcessRequestsAsync_MultipleInvalidFloorRequests() {
            // Arrange
            int startingFloor = 1;
            int validFloor = 5;
            int[] targetFloors = new int[] { validFloor, -3, 1337 };
            int requestDelay = 9001;
            var cts = new CancellationTokenSource();
            Elevator elevator = new Elevator(startingFloor);
            Task elevatorTask = elevator.ProcessRequestsAsync(cts.Token);

            // Act
            foreach (var floor in targetFloors) {
                elevator.CallElevator(floor);
            }
            await Task.Delay(requestDelay);

            cts.Cancel();

            try {
                await elevatorTask;
            }
            catch (OperationCanceledException) {
                Console.WriteLine("Elevator task was canceled.");
            }

            // Assert
            Assert.True(elevatorTask.IsCompleted);
            Assert.Equal(validFloor, elevator.currentFloor);
        }

        [Fact]
        public async Task ProcessRequestsAsync_ChangeDirection() {
            // Arrange
            int startingFloor = 5;
            int[] targetFloors = { 8, 2 };
            int requestDelay = 9000;
            var cts = new CancellationTokenSource();
            Elevator elevator = new Elevator(startingFloor);
            Task elevatorTask = elevator.ProcessRequestsAsync(cts.Token);

            // Act
            foreach (var floor in targetFloors) {
                elevator.CallElevator(floor);
            }
            await Task.Delay(requestDelay);
            cts.Cancel();

            try {
                await elevatorTask;
            }
            catch (OperationCanceledException) {
                Console.WriteLine("Elevator task was canceled.");
            }

            // Assert
            Assert.True(elevatorTask.IsCompleted);
            Assert.Equal(2, elevator.currentFloor);
        }

        [Fact]
        public async Task PressButtonInsideElevator_ValidFloor() {
            // Arrange
            int startingFloor = 1;
            int targetFloor = 5;
            int requestDelay = 5000;
            var cts = new CancellationTokenSource();
            Elevator elevator = new Elevator(startingFloor);
            Task elevatorTask = elevator.ProcessRequestsAsync(cts.Token);

            // Act
            elevator.PressButtonInsideElevator(targetFloor);
            await Task.Delay(requestDelay);
            cts.Cancel();

            try {
                await elevatorTask;
            }
            catch (OperationCanceledException) {
                Console.WriteLine("Elevator task was canceled.");
            }

            // Assert
            Assert.True(elevatorTask.IsCompleted);
            Assert.Equal(targetFloor, elevator.currentFloor);
        }

        [Fact]
        public async Task PressButtonInsideElevator_InvalidFloor() {
            // Arrange
            int startingFloor = 1;
            int invalidFloor = 1337;
            int requestDelay = 2000;
            var cts = new CancellationTokenSource();
            Elevator elevator = new Elevator(startingFloor);
            Task elevatorTask = elevator.ProcessRequestsAsync(cts.Token);

            // Act
            elevator.PressButtonInsideElevator(invalidFloor);
            await Task.Delay(requestDelay);
            cts.Cancel();

            try {
                await elevatorTask;
            }
            catch (OperationCanceledException) {
                Console.WriteLine("Elevator task was canceled.");
            }

            // Assert
            Assert.True(elevatorTask.IsCompleted);
            Assert.Equal(startingFloor, elevator.currentFloor);
        }

        [Fact]
        public async Task CancelFloorRequest_ValidRequest() {
            // Arrange
            int startingFloor = 1;
            int targetFloor = 5;
            int requestDelay = 2000;
            var cts = new CancellationTokenSource();
            Elevator elevator = new Elevator(startingFloor);
            Task elevatorTask = elevator.ProcessRequestsAsync(cts.Token);

            // Act
            elevator.CallElevator(targetFloor);
            elevator.CancelFloorRequest(targetFloor);
            await Task.Delay(requestDelay);
            cts.Cancel();
            try {
                await elevatorTask;
            }
            catch (OperationCanceledException) {
                Console.WriteLine("Elevator task was canceled.");
            }

            // Assert
            Assert.True(elevatorTask.IsCompleted);
            Assert.Equal(startingFloor, elevator.currentFloor);
        }
    }
}