interface ImageDimensions extends Size {
    renderHeight: number;
    renderWidth: number;
}

interface Size {
    height: number;
    width: number;
}

interface CropperSpecs {
    x1: number;
    x2: number;
    y1: number;
    y2: number;

    w: number;
    h: number;
}

interface CropperSettings {
    outputFormat: string;
    quality: number;
    backgroundColor: string;
    resizeTo: Size;
}

interface DotnetRef {
    invokeMethodAsync: (methodName: string, ...args: any[]) => Promise<any>;
}

export class Cropper {
    constructor(
        private _dotnetRef: DotnetRef,
        private _rootElement: HTMLElement
    ) {
        window.addEventListener("resize", this.InvokeSetRenderSize);
    }

    private InvokeStopMove = () => {
        this._dotnetRef.invokeMethodAsync('StopDrag');
        this.RemoveDragListeners();
    };

    public InvokeMove = (e: any) => {
        const v = {
            x: (e.touches && e.touches[0] && e.touches[0].clientX) || e.clientX || 0,
            y: (e.touches && e.touches[0] && e.touches[0].clientY) || e.clientY || 0,
        }

        this._dotnetRef.invokeMethodAsync('Dragging', v);

        if (e.preventDefault)
            e.preventDefault();
        if (e.stopPropagation)
            e.stopPropagation();
    };

    private InvokeSetRenderSize = () => {
        const srcImage = this._rootElement.getElementsByClassName("source-image")[0] as HTMLImageElement;
        if (srcImage) {
            this._dotnetRef.invokeMethodAsync('SetRenderSize', {
                Height: srcImage.offsetHeight,
                Width: srcImage.offsetWidth
            });
        }
    };


    public AddDragListeners = () => {
        document.addEventListener("mousemove", this.InvokeMove);
        document.addEventListener("touchmove", this.InvokeMove);

        document.addEventListener("mouseup", this.InvokeStopMove);
        document.addEventListener("touchend", this.InvokeStopMove);
    }

    private RemoveDragListeners = () => {
        document.removeEventListener("mousemove", this.InvokeMove);
        document.removeEventListener("touchmove", this.InvokeMove);

        document.removeEventListener("mouseup", this.InvokeStopMove);
        document.removeEventListener("touchend", this.InvokeStopMove);
    };

    public Dispose = () => {
        window.removeEventListener("resize", this.InvokeSetRenderSize);
    };

    public getDimensions = (element$: HTMLImageElement): ImageDimensions | undefined => {
        if (!element$)
            return;

        return {
            height: element$.naturalHeight,
            width: element$.naturalWidth,

            renderHeight: element$.offsetHeight,
            renderWidth: element$.offsetWidth,
        };
    };

    public crop = (sourceImg$: HTMLImageElement, settings: CropperSettings, cropSpecs: CropperSpecs): string | undefined => {
        const imgCanvas = document.createElement('canvas');
        const imgContext = imgCanvas.getContext('2d');

        imgCanvas.height = Math.max(cropSpecs.h, settings.resizeTo.height);
        imgCanvas.width = Math.max(cropSpecs.w, settings.resizeTo.width);

        if (!imgContext) {
            return;
        }

        imgContext.drawImage(sourceImg$,
            cropSpecs.x1, cropSpecs.y1, cropSpecs.w, cropSpecs.h,
            0, 0, cropSpecs.w, cropSpecs.h)

        const scale = settings.resizeTo.width / cropSpecs.w;
        this.scale(imgCanvas, cropSpecs.w, cropSpecs.h, scale);

        const resultCanvas = document.createElement('canvas');

        resultCanvas.height = settings.resizeTo.height;
        resultCanvas.width = settings.resizeTo.width;

        const resultContext = resultCanvas.getContext('2d');
        if (!resultContext) {
            return;
        }

        resultContext?.drawImage(imgCanvas,
            0, 0, settings.resizeTo.width, settings.resizeTo.height,
            0, 0, settings.resizeTo.width, settings.resizeTo.height
        );

        return resultCanvas.toDataURL('image/' + settings.outputFormat, settings.quality);
    }

    private scale = (canvas: HTMLCanvasElement,
        sourceWidth: number, sourceHeight: number, scale: number) => {

        if (scale === 1.0)
            return;

        let context = canvas.getContext('2d');
        if (!context) {
            return;
        }

        let currentScale = 1;
        const scaleStep = scale < 1 ? 0.75 : 2;

        while (
            (scale < 1 && currentScale * scaleStep > scale) ||
            (scale > 1 && currentScale * scaleStep < scale)
        ) {
            const nextScale = currentScale * scaleStep;

            context?.drawImage(canvas,
                0, 0,
                Math.round(sourceWidth * currentScale), Math.round(sourceHeight * currentScale),
                0, 0,
                Math.round(sourceWidth * nextScale), Math.round(sourceHeight * nextScale)
            );

            currentScale = nextScale;
        }

        context.drawImage(canvas,
            0, 0,
            Math.round(sourceWidth * currentScale), Math.round(sourceHeight * currentScale),
            0, 0,
            Math.round(sourceWidth * scale), Math.round(sourceHeight * scale)
        );

        return canvas;
    }
}

export function createCropper(_dotnetRef: DotnetRef, _rootElement: HTMLElement) {
    return new Cropper(_dotnetRef, _rootElement);
}
