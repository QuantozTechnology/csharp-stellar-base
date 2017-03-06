namespace :xdr do

  # As Hayashi adds more .x files, we'll need to update this array
  # Prior to launch, we should be separating our .x files into a separate
  # repo, and should be able to improve this integration.
  HAYASHI_XDR = [
                 "xdr/Stellar-types.x",
                 "xdr/Stellar-ledger-entries.x",
                 "xdr/Stellar-transaction.x",
                 "xdr/Stellar-ledger.x",
                 "xdr/Stellar-overlay.x",
                 "xdr/Stellar-SCP.x",
                ]

  LOCAL_XDR_PATHS = HAYASHI_XDR.map{ |src| "xdr/" + File.basename(src) }

  task :update => [:generate]

  task :generate do
    require "pathname"
    require "xdrgen"
    require "fileutils"
    FileUtils.rm_rf "generated"

    compilation = Xdrgen::Compilation.new(
      LOCAL_XDR_PATHS,
      output_dir: "src/generated",
      namespace:  "Stellar.Generated",
      language:   :Dotnet
    )
    compilation.compile
  end
end
