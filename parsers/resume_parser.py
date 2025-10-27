"""
Advanced Resume Parser using spaCy and NLTK
Extracts: Skills, Experience, Education, Certifications, Contact Info
"""

import spacy
import re
from typing import Dict, List, Optional
from dataclasses import dataclass
import PyPDF2
import docx

@dataclass
class ParsedResume:
    """Structured resume data"""
    name: Optional[str] = None
    email: Optional[str] = None
    phone: Optional[str] = None
    skills: List[str] = None
    experience: List[Dict] = None
    education: List[Dict] = None
    certifications: List[str] = None
    summary: Optional[str] = None
    raw_text: str = ""
    
    def __post_init__(self):
        self.skills = self.skills or []
        self.experience = self.experience or []
        self.education = self.education or []
        self.certifications = self.certifications or []

class AdvancedResumeParser:
    """Parse resumes with NLP and pattern matching"""
    
    def __init__(self, use_spacy: bool = True):
        self.use_spacy = use_spacy
        if use_spacy:
            try:
                self.nlp = spacy.load("en_core_web_sm")
            except:
                print("spaCy model not found. Run: python -m spacy download en_core_web_sm")
                self.nlp = None
        else:
            self.nlp = None
        
        # Comprehensive skill database (expand as needed)
        self.skill_patterns = self._load_skill_patterns()
    
    def _load_skill_patterns(self) -> Dict[str, List[str]]:
        """Load categorized skill patterns"""
        return {
            'programming': [
                'python', 'java', 'javascript', 'typescript', 'c\\+\\+', 'c#', 'ruby', 
                'go', 'rust', 'swift', 'kotlin', 'scala', 'r', 'matlab', 'perl', 'php'
            ],
            'ml_ai': [
                'machine learning', 'deep learning', 'neural networks', 'nlp', 
                'natural language processing', 'computer vision', 'reinforcement learning',
                'tensorflow', 'pytorch', 'keras', 'scikit-learn', 'opencv', 'yolo',
                'transformers', 'bert', 'gpt', 'llm', 'large language model'
            ],
            'data': [
                'sql', 'nosql', 'mongodb', 'postgresql', 'mysql', 'redis', 'elasticsearch',
                'data analysis', 'data visualization', 'pandas', 'numpy', 'matplotlib',
                'seaborn', 'plotly', 'tableau', 'power bi', 'excel', 'spark', 'hadoop'
            ],
            'cloud_devops': [
                'aws', 'azure', 'gcp', 'google cloud', 'docker', 'kubernetes', 'jenkins',
                'ci/cd', 'terraform', 'ansible', 'linux', 'bash', 'git', 'github actions'
            ],
            'web': [
                'react', 'angular', 'vue.js', 'node.js', 'express', 'django', 'flask',
                'fastapi', 'spring boot', 'html', 'css', 'sass', 'webpack', 'rest api',
                'graphql', 'microservices'
            ],
            'soft_skills': [
                'leadership', 'communication', 'teamwork', 'problem solving', 'agile',
                'scrum', 'project management', 'collaboration', 'mentoring'
            ]
        }
    
    def read_pdf(self, file_path: str) -> str:
        """Extract text from PDF"""
        text = ""
        try:
            with open(file_path, 'rb') as file:
                pdf_reader = PyPDF2.PdfReader(file)
                for page in pdf_reader.pages:
                    text += page.extract_text()
        except Exception as e:
            print(f"Error reading PDF: {e}")
        return text
    
    def read_docx(self, file_path: str) -> str:
        """Extract text from DOCX"""
        text = ""
        try:
            doc = docx.Document(file_path)
            for paragraph in doc.paragraphs:
                text += paragraph.text + "\n"
        except Exception as e:
            print(f"Error reading DOCX: {e}")
        return text
    
    def extract_contact_info(self, text: str) -> Dict[str, Optional[str]]:
        """Extract name, email, phone using regex"""
        contact = {
            'name': None,
            'email': None,
            'phone': None
        }
        
        # Email pattern
        email_pattern = r'\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b'
        email_match = re.search(email_pattern, text)
        if email_match:
            contact['email'] = email_match.group(0)
        
        # Phone pattern (US format)
        phone_pattern = r'(\+?\d{1,3}[-.\s]?)?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}'
        phone_match = re.search(phone_pattern, text)
        if phone_match:
            contact['phone'] = phone_match.group(0)
        
        # Name extraction (first line heuristic or NER)
        if self.nlp:
            doc = self.nlp(text[:500])  # Check first 500 chars
            for ent in doc.ents:
                if ent.label_ == "PERSON":
                    contact['name'] = ent.text
                    break
        
        if not contact['name']:
            # Fallback: first line if it looks like a name
            first_line = text.split('\n')[0].strip()
            if len(first_line.split()) <= 4 and len(first_line) < 50:
                contact['name'] = first_line
        
        return contact
    
    def extract_skills(self, text: str) -> List[str]:
        """Extract skills using pattern matching"""
        text_lower = text.lower()
        found_skills = []
        
        for category, skills in self.skill_patterns.items():
            for skill in skills:
                # Match whole word boundaries
                pattern = r'\b' + re.escape(skill) + r'\b'
                if re.search(pattern, text_lower):
                    # Clean up the skill name (remove regex escapes)
                    clean_skill = skill.replace('\\+\\+', '++').replace('\\', '')
                    found_skills.append(clean_skill)
        
        return list(set(found_skills))
    
    def extract_experience(self, text: str) -> List[Dict]:
        """Extract work experience entries"""
        experience = []
        
        # Pattern for experience sections
        exp_patterns = [
            r'(?:work\s+experience|professional\s+experience|employment\s+history)(.*?)(?=education|skills|certifications|projects|$)',
            r'(?:^|\n)([A-Z][A-Za-z\s&,]+(?:Inc|LLC|Corp|Ltd)?)\s*[|\n]?\s*([A-Za-z\s]+)\s*[|\n]?\s*(\d{4}|\w+\s+\d{4})\s*[-–—]\s*(\d{4}|present|current)',
        ]
        
        for pattern in summary_patterns:
            match = re.search(pattern, text, re.IGNORECASE | re.DOTALL)
            if match:
                summary = match.group(1).strip()
                # Limit to first 500 chars
                return summary[:500] if summary else None
        
        return None
    
    def parse(self, text: str) -> ParsedResume:
        """Parse resume text into structured data"""
        
        # Extract all components
        contact = self.extract_contact_info(text)
        skills = self.extract_skills(text)
        experience = self.extract_experience(text)
        education = self.extract_education(text)
        certifications = self.extract_certifications(text)
        summary = self.extract_summary(text)
        
        return ParsedResume(
            name=contact['name'],
            email=contact['email'],
            phone=contact['phone'],
            skills=skills,
            experience=experience,
            education=education,
            certifications=certifications,
            summary=summary,
            raw_text=text
        )
    
    def parse_file(self, file_path: str) -> ParsedResume:
        """Parse resume from file (PDF or DOCX)"""
        if file_path.lower().endswith('.pdf'):
            text = self.read_pdf(file_path)
        elif file_path.lower().endswith('.docx'):
            text = self.read_docx(file_path)
        else:
            raise ValueError("Unsupported file format. Use PDF or DOCX.")
        
        return self.parse(text)


# Example usage
if __name__ == "__main__":
    parser = AdvancedResumeParser(use_spacy=False)
    
    # Test with sample text
    sample_resume = """
    John Doe
    john.doe@email.com | (555) 123-4567
    
    PROFESSIONAL SUMMARY
    Senior Software Engineer with 5+ years of experience in machine learning and data science.
    
    SKILLS
    Python, TensorFlow, PyTorch, AWS, Docker, Kubernetes, SQL, React, Node.js
    
    WORK EXPERIENCE
    Tech Company Inc | Senior ML Engineer | 2020 - Present
    - Developed NLP models for customer service automation
    - Improved model accuracy by 25%
    
    EDUCATION
    Master of Science in Computer Science
    MIT | 2018
    
    CERTIFICATIONS
    - AWS Certified Solutions Architect
    - TensorFlow Developer Certificate
    """
    
    result = parser.parse(sample_resume)
    print(f"Name: {result.name}")
    print(f"Email: {result.email}")
    print(f"Skills: {result.skills}")
    print(f"Experience: {len(result.experience)} entries")
    print(f"Education: {len(result.education)} entries") in exp_patterns:
            matches = re.finditer(pattern, text, re.IGNORECASE | re.DOTALL)
            for match in matches:
                if len(match.groups()) >= 4:
                    experience.append({
                        'company': match.group(1).strip(),
                        'title': match.group(2).strip(),
                        'start_date': match.group(3).strip(),
                        'end_date': match.group(4).strip(),
                        'description': ''
                    })
        
        return experience
    
    def extract_education(self, text: str) -> List[Dict]:
        """Extract education entries"""
        education = []
        
        # Degree patterns
        degree_patterns = [
            r'(bachelor|master|phd|doctorate|associate|b\.s\.|m\.s\.|b\.a\.|m\.a\.|mba)\.?\s+(?:of\s+)?(?:science|arts|engineering|business)?\s+(?:in\s+)?([A-Za-z\s]+)',
            r'([A-Z][a-z]+\s+(?:University|College|Institute))\s*(?:[,|\n])?\s*(\d{4})?'
        ]
        
        for pattern in degree_patterns:
            matches = re.finditer(pattern, text, re.IGNORECASE)
            for match in matches:
                education.append({
                    'degree': match.group(1).strip() if len(match.groups()) > 0 else '',
                    'field': match.group(2).strip() if len(match.groups()) > 1 else '',
                    'institution': '',
                    'year': ''
                })
        
        return education[:3]  # Limit to top 3
    
    def extract_certifications(self, text: str) -> List[str]:
        """Extract certifications"""
        cert_keywords = [
            'aws certified', 'azure', 'gcp', 'pmp', 'cissp', 'comptia',
            'certified', 'certification', 'certificate'
        ]
        
        certifications = []
        lines = text.split('\n')
        
        for line in lines:
            line_lower = line.lower()
            for keyword in cert_keywords:
                if keyword in line_lower and len(line.strip()) < 100:
                    certifications.append(line.strip())
                    break
        
        return list(set(certifications))[:5]
    
    def extract_summary(self, text: str) -> Optional[str]:
        """Extract professional summary/objective"""
        summary_patterns = [
            r'(?:professional\s+summary|summary|objective|about\s+me)(.*?)(?=experience|education|skills|$)',
        ]
        
        for pattern