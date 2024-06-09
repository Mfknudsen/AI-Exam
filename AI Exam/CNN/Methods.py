import os
import pandas as pd
import numpy as np
from matplotlib.image import imread
from skimage.transform import resize
import matplotlib.pyplot as plt
import seaborn as sns

def load_images(img_dir):
    filenames = os.listdir(img_dir)
    labels = [x.split("_")[0] for x in filenames]
    img = [imread(img_dir + x) for x in filenames]

    data = pd.DataFrame({"filename": filenames, "label": labels, "img": img})

    for index, row in data.iterrows():
        img = imread(img_dir + row["filename"])
        img = resize(img, (80,80))
        data.loc[index, "img"] = img

    data = data.reset_index()

    return data

def plot_sample_images(data, samples_per_label=2):
    unique_labels = data['label'].unique()
    fig, axes = plt.subplots(len(unique_labels), samples_per_label, figsize=(samples_per_label * 3, len(unique_labels) * 3))
    
    for i, label in enumerate(unique_labels):
        label_data = data[data['label'] == label]
        sample_images = label_data.sample(n=samples_per_label)
        
        for j, (_, row) in enumerate(sample_images.iterrows()):
            ax = axes[i, j] if len(unique_labels) > 1 else axes[j]
            ax.imshow(row['img'])
            ax.set_title(f"{label}")
            ax.axis('off')
    
    plt.tight_layout()
    plt.show()

def plot_model_metrics(model):
    fig, axs = plt.subplots(1, 2, figsize=(12, 5))

    # Plot Loss
    axs[0].plot(model.history['loss'])
    axs[0].plot(model.history['val_loss'])
    axs[0].set_title('Model Loss')
    axs[0].set_ylabel('Loss')
    axs[0].set_xlabel('Epoch')
    axs[0].legend(['Train', 'Val'], loc='upper right')

    # Plot Accuracy
    axs[1].plot(model.history['accuracy'])
    axs[1].plot(model.history['val_accuracy'])
    axs[1].set_title('Model Accuracy')
    axs[1].set_ylabel('Accuracy')
    axs[1].set_xlabel('Epoch')
    axs[1].legend(['Train', 'Val'], loc='lower right')

    plt.show()

def plot_confusion_matrix(cm, generator):
    plt.figure(figsize=(10, 8))
    sns.heatmap(cm, annot=True, fmt='d', cmap='Blues', xticklabels=generator.class_indices.keys(), yticklabels=generator.class_indices.keys())
    plt.xlabel('Predicted Label')
    plt.ylabel('True Label')
    plt.title('Confusion Matrix')
    plt.show()

def plot_images_with_predictions(generator, model, num_images=25):
    x, y = next(generator)
    y_true = np.argmax(y, axis=1)
    y_pred = np.argmax(model.predict(x), axis=1)
    
    plt.figure(figsize=(15, 15))
    for i in range(num_images):
        plt.subplot(5, 5, i + 1)
        plt.imshow(x[i])
        plt.title(f"True: {list(generator.class_indices.keys())[y_true[i]]}\nPred: {list(generator.class_indices.keys())[y_pred[i]]}")
        plt.axis('off')
    plt.tight_layout()
    plt.show()

def plot_mismatched_images(image_dir, test_df, misclassified_indices):
    num_images_to_plot = min(5, len(misclassified_indices))
    if num_images_to_plot == 0:
        print("No misclassified images found.")
        return
    
    for i in range(num_images_to_plot):
        misclassified_index = misclassified_indices[i]
        # Retrieve the filename of the misclassified image
        misclassified_filename = test_df.iloc[misclassified_index]['filename']
        # Construct the full file path
        misclassified_image_path = os.path.join(image_dir, misclassified_filename)
        # Load and plot the misclassified image
        misclassified_image = plt.imread(misclassified_image_path)
        plt.subplot(1, num_images_to_plot, i+1)
        plt.imshow(misclassified_image)
        plt.title(f"Misclassified Image {i+1}")
        plt.axis('off')
    
    plt.tight_layout()
    plt.show()