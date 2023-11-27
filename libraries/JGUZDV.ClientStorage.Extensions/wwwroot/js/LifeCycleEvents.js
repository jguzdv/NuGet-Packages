export function addLifeCycleEvents(dotNet) {
    document.addEventListener("visibilitychange", function () {
        if (document.visibilityState === "visible") {
            dotNet.invokeMethodAsync("TriggerResumed")
        } else {
            dotNet.invokeMethodAsync("TriggerStopped")
        }
    });
}