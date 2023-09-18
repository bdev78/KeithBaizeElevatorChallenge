namespace KeithBaizeElevatorChallenge.Models {
    public enum ElevatorDirection {
        Up,
        Down,
        None
    }

    public class Elevator {
        public int currentFloor { get; private set; }
        private readonly int maxFloor = 10;
        private readonly int minFloor = 1;
        private HashSet<int> floorRequests;
        private HashSet<int> exitRequests;
        private readonly object lockObject;
        public ElevatorDirection direction { get; private set; }

        public Elevator(int currentFloor) {
            this.currentFloor = currentFloor;
            floorRequests = new HashSet<int>();
            exitRequests = new HashSet<int>();
            lockObject = new object();
            direction = ElevatorDirection.None;
        }

        private bool IsValidTargetFloor(int targetFloor) {
            return targetFloor >= minFloor && targetFloor <= maxFloor;
        }

        public void CallElevator(int floor) {
            lock (lockObject) {
                if (IsValidTargetFloor(floor)) {
                    floorRequests.Add(floor);
                    Console.WriteLine($"Call received from floor {floor}");
                }
                else {
                    Console.WriteLine($"Invalid call. Ignored floor {floor}.");
                }
            }
        }

        public void PressButtonInsideElevator(int floor) {
            lock (lockObject) {
                if (IsValidTargetFloor(floor)) {
                    exitRequests.Add(floor);
                    Console.WriteLine($"Destination floor {floor} selected.");
                }
                else {
                    Console.WriteLine($"Invalid button press inside elevator. Ignored floor {floor}.");
                }
            }
        }

        public async Task ProcessRequestsAsync(CancellationToken cancellationToken) {
            while (!cancellationToken.IsCancellationRequested) {
                int? targetFloor = null;

                lock (lockObject) {
                    if (floorRequests.Count > 0 || exitRequests.Count > 0) {
                        var allRequests = floorRequests.Union(exitRequests).ToList();

                        var aboveCurrent = allRequests.Any(f => f > currentFloor);
                        var belowCurrent = allRequests.Any(f => f < currentFloor);

                        if (direction == ElevatorDirection.Up && !aboveCurrent) {
                            direction = ElevatorDirection.Down;
                        }
                        else if (direction == ElevatorDirection.Down && !belowCurrent) {
                            direction = ElevatorDirection.Up;
                        }

                        if (direction == ElevatorDirection.Up) {
                            targetFloor = allRequests.Where(f => f > currentFloor).OrderBy(f => f).FirstOrDefault();
                        }
                        else if (direction == ElevatorDirection.Down) {
                            targetFloor = allRequests.Where(f => f < currentFloor).OrderByDescending(f => f).FirstOrDefault();
                        }

                        if (!targetFloor.HasValue) {
                            targetFloor = allRequests.OrderBy(f => Math.Abs(currentFloor - f)).FirstOrDefault();
                        }
                    }
                }

                if (targetFloor.HasValue && IsValidTargetFloor(targetFloor.Value)) {
                    await MoveToFloorAsync(targetFloor.Value);
                    lock (lockObject) {
                        floorRequests.Remove(targetFloor.Value);
                        exitRequests.Remove(targetFloor.Value);
                    }
                }
                else {
                    await Task.Delay(1000, cancellationToken);
                }
            }
        }


        public async Task MoveToFloorAsync(int targetFloor) {
            if (targetFloor == currentFloor) {
                return;
            }
            else if (!IsValidTargetFloor(targetFloor)) {
                Console.WriteLine("Invalid target floor. Ignoring request.");
                return;
            }

            direction = targetFloor > currentFloor ? ElevatorDirection.Up : ElevatorDirection.Down;

            await Task.Delay(Math.Abs(currentFloor - targetFloor) * 1000);

            currentFloor = targetFloor;

            Console.WriteLine($"Elevator has reached floor {currentFloor}");
            await Task.Delay(1000);
        }

        public void CancelFloorRequest(int floor) {
            lock (lockObject) {
                if (floor >= minFloor && floor <= maxFloor) {
                    floorRequests.Remove(floor);
                    exitRequests.Remove(floor);
                }
            }
        }
    }
}
