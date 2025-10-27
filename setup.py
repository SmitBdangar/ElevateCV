from setuptools import setup, find_packages
setup(
    name="saiqen",
    version="0.1.0",
    packages=find_packages(),
    install_requires=[
        'streamlit>=1.28.0',
        'openai>=1.3.0',
        'numpy>=1.24.0',
        'scikit-learn>=1.3.0',
        'python-docx>=1.0.0',
        'PyPDF2>=3.0.0',
        'nltk>=3.8.0',
        'python-dotenv>=1.0.0',
        'sentence-transformers>=2.2.0',
    ]
)