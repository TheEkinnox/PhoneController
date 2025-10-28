#import <AVFoundation/AVFoundation.h>
#import <UIKit/UIKit.h>

@interface QRScanner : NSObject <AVCaptureMetadataOutputObjectsDelegate>
@property (nonatomic, strong) AVCaptureSession *session;
@end

@implementation QRScanner

static QRScanner *instance = nil;

+ (instancetype)sharedInstance {
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        instance = [[self alloc] init];
    });
    return instance;
}

- (void)startScanning {
    if (self.session) return; // Already running
    
    AVCaptureDevice *device = [AVCaptureDevice defaultDeviceWithMediaType:AVMediaTypeVideo];
    if (!device) return;

    NSError *error = nil;
    AVCaptureDeviceInput *input = [AVCaptureDeviceInput deviceInputWithDevice:device error:&error];
    if (error) return;

    AVCaptureMetadataOutput *output = [[AVCaptureMetadataOutput alloc] init];
    [output setMetadataObjectsDelegate:self queue:dispatch_get_main_queue()];

    self.session = [[AVCaptureSession alloc] init];
    [self.session addInput:input];
    [self.session addOutput:output];

    if ([[output availableMetadataObjectTypes] containsObject:AVMetadataObjectTypeQRCode]) {
        output.metadataObjectTypes = @[AVMetadataObjectTypeQRCode];
    }

    // 🔇 No preview layer created — camera runs silently in background
    [self.session startRunning];
}

- (void)stopScanning {
    if (self.session) {
        [self.session stopRunning];
        self.session = nil;
    }
}

- (void)captureOutput:(AVCaptureOutput *)output
didOutputMetadataObjects:(NSArray<__kindof AVMetadataObject *> *)metadataObjects
       fromConnection:(AVCaptureConnection *)connection {

    for (AVMetadataObject *metadata in metadataObjects) {
        if ([metadata.type isEqualToString:AVMetadataObjectTypeQRCode]) {
            NSString *qrValue = [(AVMetadataMachineReadableCodeObject *)metadata stringValue];
            if (qrValue) {
                UnitySendMessage("QRScannerReceiver", "OnQRScanned", [qrValue UTF8String]);
            }
        }
    }
}

@end

// Unity-accessible C functions
extern "C" {
    void StartQRScanner() {
        [[QRScanner sharedInstance] startScanning];
    }

    void StopQRScanner() {
        [[QRScanner sharedInstance] stopScanning];
    }
}
